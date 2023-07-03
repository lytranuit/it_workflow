import repository from "../service/repository";

const resoure = "department";
export default {
  get() {
    return repository.get(`/v1/${resoure}/get`).then((res) => res.data);
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
  saveorder(items) {
    return repository
      .post(
        `/v1/${resoure}/saveorder`,
        { data: items },
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => res.data);
  },
};
