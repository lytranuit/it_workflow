import repository from "../service/repository";

const resoure = "processgroup";
export default {
  table(params) {
    return repository
      .post(`/v1/${resoure}/table`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  remove(items) {
    return repository
      .post(`/v1/${resoure}/remove`, items, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  save(items) {
    return repository
      .post(`/v1/${resoure}/save`, items, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
};
