#!/usr/bin/env bash

# Usage:
# ./count_loc.sh /path/to/project

set -e

TARGET_DIR="${1:-.}"

if [[ ! -d "$TARGET_DIR" ]]; then
    echo "Error: '$TARGET_DIR' is not a directory"
    exit 1
fi

total=0

while IFS= read -r -d '' file; do
    lines=$(wc -l < "$file")
    total=$((total + lines))

    printf "%8d  %s\n" "$lines" "$file"
done < <(find "$TARGET_DIR" -type f -print0)

echo "-----------------------------"
echo "Total lines: $total"