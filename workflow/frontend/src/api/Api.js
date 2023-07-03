import repository from "../service/repository";

const resoure = "api";
export default {
  process(id) {
    return repository
      .get(`/v1/${resoure}/process`, { params: { id: id } })
      .then((res) => res.data);
  },
  department() {
    return repository.get(`/v1/${resoure}/department`).then((res) => res.data);
  },
  employee() {
    return repository.get(`/v1/${resoure}/employee`).then((res) => res.data);
  },
  processgroup() {
    return repository
      .get(`/v1/${resoure}/processgroup`)
      .then((res) => res.data);
  },
  saveprocess(params) {
    return repository
      .post(`/v1/${resoure}/saveprocess`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
};
