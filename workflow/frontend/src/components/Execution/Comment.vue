<template>
  <div class="card">
    <div class="card-header">
      <b>Thảo luận</b>
    </div>
    <div class="card-body">
      <form id="binhluan" enctype="multipart/form-data">
        <input name="execution_id" :value="model.id" type="hidden" />
        <textarea
          required
          name="comment"
          style="
            padding: 10px 20px;
            border: 1px solid #d5d5d5;
            width: 100%;
            border-radius: 5px;
          "
          rows="2"
          placeholder="Bình luận?"
        ></textarea>
        <div>
          <div class="float-right">
            <button
              class="btn btn-sm btn-gradient-primary text-light px-4 mb-0"
              @click="add_comment"
            >
              Bình luận
            </button>
          </div>
          <div class="d-inline-block">
            <input
              name="file[]"
              multiple
              type="file"
              class="dropify"
              data-height="75"
              data-max-file-size="10M"
            />
          </div>
        </div>
      </form>
      <hr />
      <ul class="list-unstyled" id="comment_box">
        <li
          class="media comment_box my-2"
          :data-read="comment.is_read"
          v-for="(comment, index) in comments"
        >
          <img
            class="mr-3 rounded-circle"
            :src="comment.user.image_url"
            width="50"
            alt=""
          />
          <div class="media-body border-bottom" style="display: grid">
            <h5 class="mt-0 mb-1" style="font-size: 14px;">
              {{ comment.user.fullName }}
              <small class="text-muted">
                -
                {{ formatDate(comment.created_at, "HH:mm DD/MM/YYYY") }}</small
              >
            </h5>
            <div class="mb-2" style="white-space: pre-wrap">
              {{ comment.comment }}
            </div>
            <div class="mb-2 attach_file file-box-content">
              <div class="file-box" v-for="(file, index1) in comment.files">
                <a
                  :href="file.url"
                  :download="file.name"
                  class="download-icon-link"
                >
                  <i class="dripicons-download file-download-icon"></i>
                </a>
                <div class="text-center">
                  <i class="far fa-file text-danger"></i>
                  <h6 class="text-truncate" :title="file.name">
                    {{ file.name }}
                  </h6>
                  <small class="text-muted">{{ file.ext }}</small>
                </div>
              </div>
            </div>
          </div>
        </li>
      </ul>
      <div class="text-center load_more" @click="getComments()">
        <button class="btn btn-primary btn-sm px-5">Xem thêm bình luận</button>
      </div>
    </div>
  </div>
</template>
<script>
import Api from "../../api/Api";
import { formatDate } from "../../utilities/util";
export default {
  components: {},
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      comments: [],
      from_id: null,
    };
  },
  mounted() {
    this.getComments();
    // $(".dropify").dropify();
  },
  methods: {
    formatDate: formatDate,
    async getComments() {
      /// Lấy comments
      var execution_id = this.model.id;
      var from_id;
      if (this.comments.length > 0) {
        from_id = this.comments[this.comments.length - 1].id;
      }
      var ress = await Api.morecomment(execution_id, from_id);
      var comments = ress.comments;
      if (comments.length == 10) {
        comments.pop();
      } else {
        $(".load_more").remove();
      }
      this.comments = this.comments.concat(comments);
    },
    async add_comment(e) {
      e.preventDefault();
      var that = this;
      var comment = $("[name=comment]").val();
      var files = $("[name='file[]']")[0].files;
      //console.log(files);
      //return false;
      if (comment == "" && !files.length) {
        alert("Mời nhập bình luận!");
        return false;
      }
      var form = $("#binhluan")[0];
      var formData = new FormData(form);

      $("#binhluan").trigger("reset");
      var result = await Api.addcomment(formData);
      if (result.success) {
        var comment = result.comment;
        that.comments.unshift(comment);
      }
    },
  },
};
</script>

<style lang="scss"></style>
