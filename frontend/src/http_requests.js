var axios = require('axios');
var url = `http://localhost:5069/ihat`;

// Obras 
module.exports.getObras = () => {
    return axios.get(`${url}/constructions`)
        .then(response => {
          console.log(response.data);
          return response.data;
        })
        .catch(error => {
          console.error('Error: ', error);
        });
}

module.exports.getOneObra = (id) => {
    return axios.get(`${url}/constructions/${id}`)
        .then(response => {
          console.log(response.data);
          return response.data;
        })
        .catch(error => {
          console.error('Error: ', error);
        });
}

module.exports.deleteOneObra = (id) => {
  return axios.delete(`${url}/constructions/${id}`)
      .then(response => {
        console.log(response.data);
        return response.data;
      })
      .catch(error => {
        console.error('Error: ', error);
      });
}

module.exports.addOneObra = (body) => {
  return axios.post(`${url}/constructions`, body)
      .then(response => {
        console.log(response.data);
        return response.data;
      })
      .catch(error => {
        console.error('Error: ', error);
      });
}

// Capacetes
module.exports.getCapacetes = () => {
  return axios.get(`${url}/helmets`)
      .then(response => {
        console.log(response.data);
        return response.data;
      })
      .catch(error => {
        console.error('Error: ', error);
      });
}

module.exports.getOneCapacete = (id) => {
  return axios.get(`${url}/helmets/${id}`)
      .then(response => {
        console.log(response.data);
        return response.data;
      })
      .catch(error => {
        console.error('Error: ', error);
      });
}

module.exports.getCapacetesFromObra = (idObra) => {
  return axios.get(`${url}/constructions/${idObra}/helmets`)
      .then(response => {
        console.log(response.data);
        return response.data;
      })
      .catch(error => {
        console.error('Error: ', error);
      });
}

/* Falta patchs, 
   POST helmets
   [HttpDelete("helmet/{idCapacete}/{idObra}")], 
   [HttpPost("helmet/obra/{idObra}/{idCapacete}")]
*/
