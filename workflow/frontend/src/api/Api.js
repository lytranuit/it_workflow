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
  ProcessGroupWithProcess() {
    return repository
      .get(`/v1/${resoure}/ProcessGroupWithProcess`)
      .then((res) => res.data);
  },
  processgroup() {
    return repository
      .get(`/v1/${resoure}/processgroup`)
      .then((res) => res.data);
  },
  ProcessVersion(id) {
    return repository
      .get(`/v1/${resoure}/ProcessVersion`, { params: { id: id } })
      .then((res) => res.data);
  },
  execution(execution_id) {
    return repository
      .get(`/v1/${resoure}/execution`, { params: { id: execution_id } })
      .then((res) => res.data);
  },
  TransitionByExecution(execution_id) {
    return repository
      .get(`/v1/${resoure}/TransitionByExecution`, {
        params: { execution_id: execution_id },
      })
      .then((res) => res.data);
  },
  ActivityByExecution(execution_id) {
    return repository
      .get(`/v1/${resoure}/ActivityByExecution`, {
        params: { execution_id: execution_id },
      })
      .then((res) => res.data);
  },
  CustomBlockByExecution(execution_id) {
    return repository
      .get(`/v1/${resoure}/CustomBlockByExecution`, {
        params: { execution_id: execution_id },
      })
      .then((res) => res.data);
  },
  HomeBadge() {
    return repository.get(`/v1/${resoure}/HomeBadge`).then((res) => res.data);
  },
  datachartDepartment() {
    return repository
      .get(`/v1/${resoure}/datachartDepartment`)
      .then((res) => res.data);
  },

  tableUser() {
    return repository
      .post(
        `/v1/${resoure}/tableUser`,
        {},
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => res.data);
  },

  tableProcess() {
    return repository
      .post(
        `/v1/${resoure}/tableProcess`,
        {},
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
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
  morecomment(execution_id, from_id) {
    return repository
      .get(`/v1/${resoure}/morecomment`, {
        params: { execution_id: execution_id, from_id: from_id },
      })
      .then((res) => res.data);
  },
  events(execution_id) {
    return repository
      .get(`/v1/${resoure}/events`, {
        params: { execution_id: execution_id },
      })
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

  updateexecution(params) {
    return repository
      .post(`/v1/${resoure}/updateexecution`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  createexecution(params) {
    return repository
      .post(`/v1/${resoure}/createexecution`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  createcustomblock(params) {
    return repository
      .post(`/v1/${resoure}/createcustomblock`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  updatecustomblock(params) {
    return repository
      .post(`/v1/${resoure}/updatecustomblock`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  createactivity(params) {
    return repository
      .post(`/v1/${resoure}/createactivity`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  updateactivity(params) {
    return repository
      .post(`/v1/${resoure}/updateactivity`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  createtransition(params) {
    return repository
      .post(`/v1/${resoure}/createtransition`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  uploadFile(params) {
    return repository
      .post(`/v1/${resoure}/uploadFile`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  addcomment(params) {
    return repository
      .post(`/v1/${resoure}/addcomment`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
  SaveSign(params) {
    return repository
      .post(`/v1/${resoure}/SaveSign`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => res.data);
  },
};
