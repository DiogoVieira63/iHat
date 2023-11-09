from flask import Flask, request, jsonify
import ifcopenshell
import ifcopenshell.draw
from bs4 import BeautifulSoup

app = Flask(__name__)

@app.route('/model2svg', methods=['POST'])
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
    svg_files = []
    for i, group in enumerate(main_groups):
        # Create a new SVG document
        # get this info from output_data
        svg_file = soup.new_tag('svg', attrs=svg.attrs)
        svg_file.append(defs)
        svg_file.append(style)
        svg_file.append(group)
        # Write the SVG document to a file
        output_file = f'{group["ifc:name"]}.svg'
        with open(output_file, 'w') as f:
            f.write(svg_file.prettify())
        svg_files.append(output_file)

    # Return the SVG files as a JSON response
    return jsonify({'svg_files': svg_files})

if __name__ == '__main__':
    app.run(debug=True)