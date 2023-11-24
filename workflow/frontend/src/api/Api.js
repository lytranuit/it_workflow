import repository from "../service/repository";

const resoure = "api";
export default {
  process(id) {
    return repository
      .get(`/v1/${resoure}/process`, { params: { id: id } })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  department() {
    return repository.get(`/v1/${resoure}/department`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  employee() {
    return repository.get(`/v1/${resoure}/employee`).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  ProcessGroupWithProcess() {
    return repository
      .get(`/v1/${resoure}/ProcessGroupWithProcess`)
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  processgroup() {
    return repository
      .get(`/v1/${resoure}/processgroup`)
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  ProcessVersion(id) {
    return repository
      .get(`/v1/${resoure}/ProcessVersion`, { params: { id: id } })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  execution(execution_id) {
    return repository
      .get(`/v1/${resoure}/execution`, { params: { id: execution_id } })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  TransitionByExecution(execution_id) {
    return repository
      .get(`/v1/${resoure}/TransitionByExecution`, {
        params: { execution_id: execution_id },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  ActivityByExecution(execution_id) {
    return repository
      .get(`/v1/${resoure}/ActivityByExecution`, {
        params: { execution_id: execution_id },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  CustomBlockByExecution(execution_id) {
    return repository
      .get(`/v1/${resoure}/CustomBlockByExecution`, {
        params: { execution_id: execution_id },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  HomeBadge(process_id = null) {
    return repository.get(`/v1/${resoure}/HomeBadge`, {
      params: { process_id: process_id },
    }).then((res) => {
      var url = "/Identity/Account/Login";
      if (res.request.responseURL.indexOf(url) != -1)
        location.reload();
      return res.data
    });;
  },
  datachartDepartment(process_id = null) {
    return repository
      .get(`/v1/${resoure}/datachartDepartment`, {
        params: { process_id: process_id },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },

  tableUser(process_id = null) {
    return repository
      .post(
        `/v1/${resoure}/tableUser`,
        { process_id: process_id },
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },

  tableProcess(process_id = null) {
    return repository
      .post(
        `/v1/${resoure}/tableProcess`,
        { process_id: process_id },
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  tableExecution(process_id = null) {
    return repository
      .post(
        `/v1/${resoure}/tableExecution`,
        { process_id: process_id },
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      )
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  saveprocess(params) {
    return repository
      .post(`/v1/${resoure}/saveprocess`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  morecomment(execution_id, from_id) {
    return repository
      .get(`/v1/${resoure}/morecomment`, {
        params: { execution_id: execution_id, from_id: from_id },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  events(execution_id) {
    return repository
      .get(`/v1/${resoure}/events`, {
        params: { execution_id: execution_id },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  saveprocess(params) {
    return repository
      .post(`/v1/${resoure}/saveprocess`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },

  updateexecution(params) {
    return repository
      .post(`/v1/${resoure}/updateexecution`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  createexecution(params) {
    return repository
      .post(`/v1/${resoure}/createexecution`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  createcustomblock(params) {
    return repository
      .post(`/v1/${resoure}/createcustomblock`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  updatecustomblock(params) {
    return repository
      .post(`/v1/${resoure}/updatecustomblock`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  createactivity(params) {
    return repository
      .post(`/v1/${resoure}/createactivity`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  updateactivity(params) {
    return repository
      .post(`/v1/${resoure}/updateactivity`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  createtransition(params) {
    return repository
      .post(`/v1/${resoure}/createtransition`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  uploadFile(params) {
    return repository
      .post(`/v1/${resoure}/uploadFile`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  addcomment(params) {
    return repository
      .post(`/v1/${resoure}/addcomment`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
  SaveSign(params) {
    return repository
      .post(`/v1/${resoure}/SaveSign`, params, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })
      .then((res) => {
        var url = "/Identity/Account/Login";
        if (res.request.responseURL.indexOf(url) != -1)
          location.reload();
        return res.data
      });;
  },
};
