import requests

url = 'http://localhost:5000/ifc2svg'  # Replace with your actual URL
file_path = 'Duplex.ifc'  # Replace with your actual file path

with open(file_path, 'rb') as f:
    files = {'ifc_file': f}
    response = requests.post(url, files=files)
    # Save the response to a file
    with open('output.zip', 'wb') as f:
        f.write(response.content)

url = 'http://localhost:5000/dxf2svg'  # Replace with your actual URL
file_path = 'piso_0.dxf'  # Replace with your actual file path

with open(file_path, 'rb') as f:
    files = {'dxf_file': f}
    response = requests.post(url, files=files)
    # Save the response to a file
    with open('output.svg', 'wb') as f:
        f.write(response.content)

