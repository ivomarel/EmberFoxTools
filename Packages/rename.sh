#!/bin/bash

olds=("Ivovdmarel" "ivovdmarel")
news=("Emberfox" "emberfox")

SCRIPT=$(basename "$0")

for i in "${!olds[@]}"; do
  old="${olds[$i]}"
  new="${news[$i]}"

  # Rename files and folders
  find . -depth -name "*$old*" ! -name "$SCRIPT" | while read f; do
    newpath="${f//$old/$new}"
    if [[ "$f" != "$newpath" ]]; then
      mv "$f" "$newpath"
    fi
  done

  # Replace inside files (excluding this script)
  grep -rl --exclude="$SCRIPT" "$old" . | xargs sed -i '' -e "s/$old/$new/g"
done