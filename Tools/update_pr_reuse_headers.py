#!/usr/bin/env python3

import subprocess
import os
import sys
import re
import argparse
import json
from datetime import datetime, timezone
from collections import defaultdict

# --- Configuration ---
LICENSE_CONFIG = {
    "mit": {"id": "MIT", "path": "LICENSES/MIT.txt"},
    "agpl": {"id": "AGPL-3.0-or-later", "path": "LICENSES/AGPLv3.txt"},
}
DEFAULT_LICENSE_LABEL = "agpl"

# Dictionary mapping file extensions to comment styles
# Format: {extension: (prefix, suffix)}
# If suffix is None, it's a single-line comment style
COMMENT_STYLES = {
    ".cs": ("//", None),
    ".yaml": ("#", None),
    ".yml": ("#", None),
    ".xaml": ("<!--", "-->"),
    ".xml": ("<!--", "-->"),
}
REPO_PATH = "."

def run_git_command(command, cwd=REPO_PATH, check=True):
    """Runs a git command and returns its output."""
    try:
        result = subprocess.run(
            command,
            capture_output=True,
            text=True,
            check=check,
            cwd=cwd,
            encoding='utf-8',
            errors='ignore'
        )
        return result.stdout.strip()
    except subprocess.CalledProcessError as e:
        if check:
            print(f"Error running git command {' '.join(command)}: {e.stderr}", file=sys.stderr)
        return None
    except FileNotFoundError:
        print("FATAL: 'git' command not found. Make sure git is installed and in your PATH.", file=sys.stderr)
        return None

def get_authors_from_git(file_path, cwd=REPO_PATH):
    """
    Gets authors and their contribution years for a specific file.
    Returns: dict like {"Author Name <email>": (min_year, max_year)}
    """
    # Always get all authors
    command = ["git", "log", "--pretty=format:%at|%an|%ae|%b", "--follow", "--", file_path]

    output = run_git_command(command, cwd=cwd, check=False)
    if not output:
        # Try to get the current user from git config as a fallback
        try:
            name_cmd = ["git", "config", "user.name"]
            email_cmd = ["git", "config", "user.email"]
            user_name = run_git_command(name_cmd, cwd=cwd, check=False)
            user_email = run_git_command(email_cmd, cwd=cwd, check=False)

            # Use current year
            current_year = datetime.now(timezone.utc).year
            if user_name and user_email and user_name.strip() != "Unknown":
                return {f"{user_name} <{user_email}>": (current_year, current_year)}
            else:
                print("Warning: Could not get current user from git config or name is 'Unknown'")
                return {}
        except Exception as e:
            print(f"Error getting git user: {e}")
        return {}

    # Process the output
    author_timestamps = defaultdict(list)
    co_author_regex = re.compile(r"^Co-authored-by:\s*(.*?)\s*<([^>]+)>", re.MULTILINE)

    for line in output.splitlines():
        if not line.strip():
            continue

        parts = line.split('|', 3)
        if len(parts) < 4:
            continue

        timestamp_str, author_name, author_email, body = parts

        try:
            timestamp = int(timestamp_str)
        except ValueError:
            continue

        # Add main author
        if author_name and author_email and author_name.strip() != "Unknown":
            author_key = f"{author_name.strip()} <{author_email.strip()}>"
            author_timestamps[author_key].append(timestamp)

        # Add co-authors
        for match in co_author_regex.finditer(body):
            co_author_name = match.group(1).strip()
            co_author_email = match.group(2).strip()
            if co_author_name and co_author_email and co_author_name.strip() != "Unknown":
                co_author_key = f"{co_author_name} <{co_author_email}>"
                author_timestamps[co_author_key].append(timestamp)

    # Convert timestamps to years
    author_years = {}
    for author, timestamps in author_timestamps.items():
        if not timestamps:
            continue
        min_ts = min(timestamps)
        max_ts = max(timestamps)
        min_year = datetime.fromtimestamp(min_ts, timezone.utc).year
        max_year = datetime.fromtimestamp(max_ts, timezone.utc).year
        author_years[author] = (min_year, max_year)

    return author_years

def parse_existing_header(content, comment_style):
    """
    Parses an existing REUSE header to extract authors and license.
    Returns: (authors_dict, license_id, header_lines)

    comment_style is a tuple of (prefix, suffix)
    """
    prefix, suffix = comment_style
    lines = content.splitlines()
    authors = {}
    license_id = None
    header_lines = []

    if suffix is None:
        # Single-line comment style (e.g., //, #)
        # Regular expressions for parsing
        copyright_regex = re.compile(f"^{re.escape(prefix)} SPDX-FileCopyrightText: (\\d{{4}}) (.+)$")
        license_regex = re.compile(f"^{re.escape(prefix)} SPDX-License-Identifier: (.+)$")

        # Find the header section
        in_header = True
        for i, line in enumerate(lines):
            if in_header:
                header_lines.append(line)

                # Check for copyright line
                copyright_match = copyright_regex.match(line)
                if copyright_match:
                    year = int(copyright_match.group(1))
                    author = copyright_match.group(2).strip()
                    authors[author] = (year, year)
                    continue

                # Check for license line
                license_match = license_regex.match(line)
                if license_match:
                    license_id = license_match.group(1).strip()
                    continue

                # Empty comment line or separator
                if line.strip() == prefix:
                    continue

                # If we get here, we've reached the end of the header
                if i > 0:  # Only if we've processed at least one line
                    header_lines.pop()  # Remove the non-header line
                    in_header = False
            else:
                break
    else:
        # Multi-line comment style (e.g., <!-- -->)
        # Regular expressions for parsing
        copyright_regex = re.compile(r"^SPDX-FileCopyrightText: (\d{4}) (.+)$")
        license_regex = re.compile(r"^SPDX-License-Identifier: (.+)$")

        # Find the header section
        in_comment = False
        for i, line in enumerate(lines):
            stripped_line = line.strip()

            # Start of comment
            if stripped_line == prefix:
                in_comment = True
                header_lines.append(line)
                continue

            # End of comment
            if stripped_line == suffix and in_comment:
                header_lines.append(line)
                break

            if in_comment:
                header_lines.append(line)

                # Check for copyright line
                copyright_match = copyright_regex.match(stripped_line)
                if copyright_match:
                    year = int(copyright_match.group(1))
                    author = copyright_match.group(2).strip()
                    authors[author] = (year, year)
                    continue

                # Check for license line
                license_match = license_regex.match(stripped_line)
                if license_match:
                    license_id = license_match.group(1).strip()
                    continue

    return authors, license_id, header_lines

def create_header(authors, license_id, comment_style):
    """
    Creates a REUSE header with the given authors and license.
    Returns: header string

    comment_style is a tuple of (prefix, suffix)
    """
    prefix, suffix = comment_style
    lines = []

    if suffix is None:
        # Single-line comment style (e.g., //, #)
        # Add copyright lines
        if authors:
            for author, (_, year) in sorted(authors.items(), key=lambda x: (x[1][1], x[0])):
                if not author.startswith("Unknown <"):
                    lines.append(f"{prefix} SPDX-FileCopyrightText: {year} {author}")
        else:
            lines.append(f"{prefix} SPDX-FileCopyrightText: Contributors to the GoobStation14 project")

        # Add separator
        lines.append(f"{prefix}")

        # Add license line
        lines.append(f"{prefix} SPDX-License-Identifier: {license_id}")
    else:
        # Multi-line comment style (e.g., <!-- -->)
        # Start comment
        lines.append(f"{prefix}")

        # Add copyright lines
        if authors:
            for author, (_, year) in sorted(authors.items(), key=lambda x: (x[1][1], x[0])):
                if not author.startswith("Unknown <"):
                    lines.append(f"SPDX-FileCopyrightText: {year} {author}")
        else:
            lines.append(f"SPDX-FileCopyrightText: Contributors to the GoobStation14 project")

        # Add separator
        lines.append("")

        # Add license line
        lines.append(f"SPDX-License-Identifier: {license_id}")

        # End comment
        lines.append(f"{suffix}")

    return "\n".join(lines)

def process_file(file_path, default_license_id):
    """
    Processes a file to add or update REUSE headers.
    Returns: True if file was modified, False otherwise
    """
    # Check file extension
    _, ext = os.path.splitext(file_path)
    comment_style = COMMENT_STYLES.get(ext)
    if not comment_style:
        print(f"Skipping unsupported file type: {file_path}")
        return False

    # Check if file exists
    full_path = os.path.join(REPO_PATH, file_path)
    if not os.path.exists(full_path):
        print(f"File not found: {file_path}")
        return False

    # Read file content
    with open(full_path, 'r', encoding='utf-8-sig', errors='ignore') as f:
        content = f.read()

    # Parse existing header if any
    existing_authors, existing_license, header_lines = parse_existing_header(content, comment_style)

    # Get all authors from git
    git_authors = get_authors_from_git(file_path)

    # Add current user to authors
    try:
        name_cmd = ["git", "config", "user.name"]
        email_cmd = ["git", "config", "user.email"]
        user_name = run_git_command(name_cmd, check=False)
        user_email = run_git_command(email_cmd, check=False)

        if user_name and user_email and user_name.strip() != "Unknown":
            # Use current year
            current_year = datetime.now(timezone.utc).year
            current_user = f"{user_name} <{user_email}>"

            # Add current user if not already present
            if current_user not in git_authors:
                git_authors[current_user] = (current_year, current_year)
                print(f"  Added current user: {current_user}")
            else:
                # Update year if necessary
                min_year, max_year = git_authors[current_user]
                git_authors[current_user] = (min(min_year, current_year), max(max_year, current_year))
        else:
            print("Warning: Could not get current user from git config or name is 'Unknown'")
    except Exception as e:
        print(f"Error getting git user: {e}")

    # Determine what to do based on existing header
    if existing_license:
        print(f"Updating existing header for {file_path} (License: {existing_license})")

        # Combine existing and git authors
        combined_authors = existing_authors.copy()
        for author, (git_min, git_max) in git_authors.items():
            if author.startswith("Unknown <"):
                continue
            if author in combined_authors:
                existing_min, existing_max = combined_authors[author]
                combined_authors[author] = (min(existing_min, git_min), max(existing_max, git_max))
            else:
                combined_authors[author] = (git_min, git_max)
                print(f"  Adding new author: {author}")

        # Create new header with existing license
        new_header = create_header(combined_authors, existing_license, comment_style)

        # Replace old header with new header
        if header_lines:
            old_header = "\n".join(header_lines)
            new_content = content.replace(old_header, new_header, 1)
        else:
            # No header found (shouldn't happen if existing_license is set)
            new_content = new_header + "\n\n" + content
    else:
        print(f"Adding new header to {file_path} (License: {default_license_id})")

        # Create new header with default license
        new_header = create_header(git_authors, default_license_id, comment_style)

        # Add header to file
        if content.strip():
            # For XML files, we need to add the header after the XML declaration if present
            prefix, suffix = comment_style
            if suffix and content.lstrip().startswith("<?xml"):
                # Find the end of the XML declaration
                xml_decl_end = content.find("?>") + 2
                xml_declaration = content[:xml_decl_end]
                rest_of_content = content[xml_decl_end:].lstrip()
                new_content = xml_declaration + "\n" + new_header + "\n\n" + rest_of_content
            else:
                new_content = new_header + "\n\n" + content
        else:
            new_content = new_header + "\n"

    # Check if content changed
    if new_content == content:
        print(f"No changes needed for {file_path}")
        return False

    # Write updated content
    with open(full_path, 'w', encoding='utf-8', newline='\n') as f:
        f.write(new_content)

    print(f"Updated {file_path}")
    return True

def main():
    parser = argparse.ArgumentParser(description="Update REUSE headers for PR files")
    parser.add_argument("--files-added", nargs="*", default=[], help="List of added files")
    parser.add_argument("--files-modified", nargs="*", default=[], help="List of modified files")
    parser.add_argument("--pr-license", default=DEFAULT_LICENSE_LABEL, help="License to use for new files")

    args = parser.parse_args()

    # Validate license
    license_label = args.pr_license.lower()
    if license_label not in LICENSE_CONFIG:
        print(f"Warning: Unknown license '{license_label}', using default: {DEFAULT_LICENSE_LABEL}")
        license_label = DEFAULT_LICENSE_LABEL

    license_id = LICENSE_CONFIG[license_label]["id"]
    print(f"Using license for new files: {license_id}")

    # Process files
    files_changed = False

    print("\n--- Processing Added Files ---")
    for file in args.files_added:
        if process_file(file, license_id):
            files_changed = True

    print("\n--- Processing Modified Files ---")
    for file in args.files_modified:
        if process_file(file, license_id):
            files_changed = True

    print("\n--- Summary ---")
    if files_changed:
        print("Files were modified")
    else:
        print("No files needed changes")

if __name__ == "__main__":
    main()
