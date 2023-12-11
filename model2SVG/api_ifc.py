from flask import Flask, request
import ifcopenshell
import ifcopenshell.draw
from bs4 import BeautifulSoup
import subprocess
import os
from flask import send_file
import shutil

app = Flask(__name__)

@app.route('/ifc2svg', methods=['POST'])
def generate_floor_plans():
    # Get the IFC file from the request
    ifc_file = request.files['ifc_file']

    # Open the IFC file using ifcopenshell
    ifc_data = ifc_file.read()

    with open('test.ifc', 'wb') as f:
        f.write(ifc_data)

    # Parse the IFC data using ifcopenshell
    ifc_file = ifcopenshell.open('test.ifc')
    
    # Generate the floor plans using ifcopenshell.draw
    draw_settings = ifcopenshell.draw.draw_settings(auto_floorplan=True,)
    output_data = ifcopenshell.draw.main(draw_settings, files=[ifc_file,], merge_projection=False)

    # Parse the SVG data using Beautiful Soup
    soup = BeautifulSoup(output_data, 'xml')
    # get atribtes of the svg file
    svg = soup.find('svg')
    defs = soup.find('defs')
    style = soup.find('style')


    # Find all main groups in the SVG file
    main_groups = soup.find_all('g', attrs={'class': 'IfcBuildingStorey'})

    # Iterate through main groups and write each group to a separate SVG file
    # Create a directory for the SVG files
    os.makedirs('svg_files', exist_ok=True)
    for i, group in enumerate(main_groups):
        # Create a new SVG document
        # get this info from output_data
        svg_file = soup.new_tag('svg', attrs=svg.attrs)
        svg_file.append(defs)
        svg_file.append(style)
        svg_file.append(group)
        # Write the SVG document to a file
        output_file = os.path.join('svg_files', f'{group["ifc:name"]}.svg')
        with open(output_file, 'w') as f:
            f.write(svg_file.prettify())
        
        subprocess.call(['inkscape','-o',output_file,'-D',output_file])

    # Zip the SVG files
    shutil.make_archive('svg_files', 'zip', 'svg_files')

    # Remove the SVG files directory
    shutil.rmtree('svg_files')
    # Remove the IFC file
    os.remove('test.ifc')

    # Return the zip file
    # remove the zip file after sending
    return send_file('svg_files.zip', as_attachment=True)


@app.route('/dxf2svg', methods=['POST'])
def dxf2svg():
    # Get the IFC file from the request
    dxf_file = request.files['dxf_file']

    # Open the dxf file using dxfopenshell
    dxf_data = dxf_file.read()

    with open('test.dxf', 'wb') as f:
        f.write(dxf_data)

    subprocess.call(['./dxf2svg.sh','test.dxf','test.svg'])

    # Return the svg file
    return send_file('test.svg', as_attachment=True)


if __name__ == '__main__':
    app.run(debug=True)