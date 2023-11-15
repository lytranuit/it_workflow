import axios from "axios";
// import { Service } from 'axios-middleware';

// const service = new Service(axios);

// service.register({
//   onRequest(config) {
//     console.log('onRequest');
//     return config;
//   },
//   onResponse(response) {
//     console.log('onResponse');
//     return response;
//   }
// });
export default axios.create({
  baseURL: import.meta.env.VITE_BASEURL,
  maxRedirects: 0
});
