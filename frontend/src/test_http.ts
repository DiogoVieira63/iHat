import {
    ObraService,
    CapaceteService,
} from './http_requests';

import type { Obra, Capacete, CapacetePost, ObraPost } from './interfaces';
  
//   ObraService.getObras()
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });

// -----------------------------------------------------------

//   ObraService.getOneObra('655655f909d614d101062c70')
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });
  
// -----------------------------------------------------------

//   CapaceteService.getCapacetes()
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });
  
// -----------------------------------------------------------

//   CapaceteService.getOneCapacete('6564d27cda6e5818692321b5')
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });
  
// -----------------------------------------------------------

//   ObraService.deleteOneObra('655a89150d41910b8b1db791')
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });
  
// -----------------------------------------------------------

//   let obra: ObraPost = {
//     Name: 'Obra2',
//     Mapa: '',
//     Status: 'Planeada'
//   }
  
//   ObraService.addOneObra(obra)
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });
  
// -----------------------------------------------------------

//   ObraService.getCapacetesFromObra('655655f909d614d101062c70')
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });

// -----------------------------------------------------------
  // params => 1st: obra; 2nd: capacete
//   ObraService.deleteCapaceteFromObra('655655f909d614d101062c70', "6564d27cda6e5818692321b5")
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });
  
// -----------------------------------------------------------

  // params => 1st: obra; 2nd: capacete
//   ObraService.addCapaceteToObra("655655f909d614d101062c70",'6564d27cda6e5818692321b5')
//     .then(() => {
//       console.log('Function executed successfully');
//     })
//     .catch((error) => {
//       console.error('Error executing getObras:', error);
//     });
  
// -----------------------------------------------------------
  
  let obra:CapacetePost = {
    NCapacete: 10,
  };
  
  CapaceteService.addOneCapacete(obra)
    .then(() => {
      console.log('Function executed successfully');
    })
    .catch((error) => {
      console.error('Error executing getObras:', error);
    });
  