#!/bin/bash

input_dxf=$1
output_svg=$2

python /usr/share/inkscape/extensions/dxf_input.py "$input_dxf" > "input.svg"

inkscape -o "$output_svg" -D "input.svg"

rm "input.svg"

echo "Conversion complete. Output saved to $output_svg"
