import repository from "../service/repository";

const resoure = "auth";
export default {
  logout() {
    return repository.post(`/v1/${resoure}/Logout`).then((res) => res.data);
  },
  TokenInfo(token) {
    return repository
      .get(`/v1/${resoure}/TokenInfo?token=${token}`)
      .then((res) => res.data);
  },
};
