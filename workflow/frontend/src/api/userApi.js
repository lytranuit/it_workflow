import repository from "../service/repository";

const resoure = "user";
export default {
  get(id) {
    return repository.get(`/v1/${resoure}/get/${id}`).then((res) => res.data);
  },
  departments() {
    return repository.get(`/v1/${resoure}/departments`).then((res) => res.data);
  },
  roles() {
    return repository.get(`/v1/${resoure}/roles`).then((res) => res.data);
  },
  create(formData) {
    return repository
      .post(`/v1/${resoure}/create`, formData)
      .then((res) => res.data);
  },
  edit(formData) {
    return repository
      .post(`/v1/${resoure}/edit`, formData)
      .then((res) => res.data);
  },
  changepassword(formData) {
    return repository
      .post(`/v1/${resoure}/changepassword`, formData)
      .then((res) => res.data);
  },
  excel() {
    return repository.post(`/v1/${resoure}/excel`).then((res) => res.data);
  },
  sync() {
    return repository.post(`/v1/${resoure}/sync`).then((res) => res.data);
  },
  table(params) {
    return repository
      .post(`/v1/${resoure}/table`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  delete(id) {
    return repository
      .post(
        `/v1/${resoure}/delete`,
        { id: id },
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => res.data);
  },
};
