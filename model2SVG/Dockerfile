# Use an official Python runtime as the base image
FROM python:3.9-slim

# Set the working directory in the container
WORKDIR /app

# Install build dependencies (if needed) and Inkscape
RUN apt-get update \
    && apt-get install -y --no-install-recommends build-essential inkscape \
    && rm -rf /var/lib/apt/lists/*

# Copy the requirements file to the working directory
COPY ./requirements.txt .

# Install Python dependencies
RUN pip install --no-cache-dir -r requirements.txt
RUN pip install gunicorn

# Copy the app.py file to the working directory
COPY app.py .

# Expose the port on which the application will run
EXPOSE 5000

# Set the command to run the application with Gunicorn
CMD ["gunicorn", "-w", "4", "-b", "0.0.0.0:5000", "app:app"]
