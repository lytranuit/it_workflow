<template>
  <div class="" id="approve">
    <ul class="nav nav-pills nav-justified mb-3" role="tablist">
      <li
        class="nav-item waves-effect waves-light"
        v-if="url != null && readonly == false"
      >
        <a
          class="nav-link"
          data-toggle="tab"
          href="#approve-sign"
          role="tab"
          aria-selected="false"
          >Ký tên</a
        >
      </li>
      <li
        class="nav-item waves-effect waves-light"
        v-if="url != null && readonly == true"
      >
        <a
          class="nav-link"
          data-toggle="tab"
          href="#viewer"
          role="tab"
          aria-selected="false"
          >Xem file</a
        >
      </li>
      <li
        class="nav-item waves-effect waves-light"
        v-for="(element, index) in blocks_approve"
      >
        <a
          class="nav-link"
          data-toggle="tab"
          :href="'#tab-' + index"
          role="tab"
          aria-selected="false"
          >{{ element.label }}</a
        >
      </li>
    </ul>
    <div class="tab-content" style="height: calc(100% - 58px)">
      <div
        class="tab-pane h-100"
        id="approve-sign"
        role="tabpanel"
        v-if="url != null && readonly == false"
      >
        <div class="card no-shadow border">
          <div class="card-body">
            <div class="row g-0">
              <div
                class="col-9"
                style="border: 5px solid #d7d7d7; border-left: 0"
              >
                <div
                  id="pdf-viewer"
                  style="height: 620px"
                  :data-url="file"
                ></div>
                <!--<PDFViewer :source="url"
                        style="height: 620px;/>-->
              </div>
              <div
                class="col-3 order-first"
                style="
                  border: 5px solid #d7d7d7;
                  height: 640px;
                  display: inline-block;
                  padding: 10px;
                "
              >
                <div class="base-title">Chữ ký</div>
                <span class="base-subtitle">Kéo chữ ký vào văn bản để ký</span>
                <div id="sign">
                  <div
                    class="signature ui-draggable ui-draggable-handle"
                    :data-id="current_user.id"
                    v-if="current_user != null"
                  >
                    <div class="d-inline-block">
                      <img
                        class="sign_image"
                        :src="current_user.image_sign"
                        style="width: 120px; height: auto"
                        alt="..."
                      />
                      <div class="sign_info" style="align-self: center">
                        <div>{{ current_user.fullName }}</div>
                        <div>Current Time</div>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="acti mt-2" v-if="listusersign.length > 0">
                  <div data-v-0eb0921e="" class="base-title">
                    Yêu cầu phê duyệt
                  </div>
                  <template v-for="(user, index) in listusersign">
                    <div class="flex-m mt-2" v-if="user.status == 1">
                      <i class="icon-warning">
                        <span
                          class="fas fa-spinner fa-spin"
                          style="transform: rotate(-45deg)"
                        ></span>
                      </i>
                      <div
                        class="user_signature time-item"
                        data-id="5a375cd2-1908-4784-9b7b-d470e2d63376"
                      >
                        <div class="item-info" style="min-height: 50px">
                          <div class="mt-0">
                            <b>{{ user.user.name }}</b> đã được yêu cầu phê
                            duyệt
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="flex-m mt-2" v-if="user.status == 2">
                      <i class="icon-success">
                        <span
                          class="fas fa-check-circle"
                          style="transform: rotate(-45deg)"
                        ></span>
                      </i>
                      <div
                        class="user_signature time-item"
                        data-id="5a375cd2-1908-4784-9b7b-d470e2d63376"
                      >
                        <div class="item-info" style="min-height: 50px">
                          <div class="mt-0">
                            <b>{{ user.user.name }}</b> đã
                            <strong class="text-success">đồng ý</strong> phê
                            duyệt
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="flex-m mt-2" v-if="user.status == 3">
                      <i class="text-white bg-danger">
                        <span
                          class="fas fa-ban"
                          style="transform: rotate(-45deg)"
                        ></span>
                      </i>
                      <div
                        class="user_signature time-item"
                        data-id="5a375cd2-1908-4784-9b7b-d470e2d63376"
                      >
                        <div class="item-info" style="min-height: 50px">
                          <div class="mt-0">
                            <b>{{ user.user.name }}</b> đã
                            <strong class="text-danger">không đồng ý</strong>
                            phê duyệt
                          </div>
                        </div>
                      </div>
                    </div>
                  </template>
                </div>
                <div class="text-center mt-2">
                  <a
                    class="btn btn-success btn-sm"
                    type="button"
                    href="#"
                    @click="require_sign(model)"
                  >
                    <i class="fas fa-plus mr-1"></i>
                    Yêu cầu ký nháy
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div
        class="tab-pane h-100"
        id="viewer"
        role="tabpanel"
        v-if="url != null && readonly == true"
      >
        <embed
          :src="url"
          style="width: 100%; height: 100%"
          type="application/pdf"
        />
      </div>
      <div
        class="tab-pane h-100"
        :id="'tab-' + index"
        role="tabpanel"
        v-for="(element, index) in blocks_approve"
      >
        <div class="bg-white py-3">
          <FormTask
            :departments="departments"
            :users="users"
            :fields="element.fields"
            readonly
          ></FormTask>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { useProcess } from "../../../stores/process";
import { useAuth } from "../../../stores/auth";
import FormTask from "./FormTask.vue";
var store_auth = useAuth();
var store = useProcess();
var thePdf;
//import PDFViewer from 'pdf-viewer-vue'
export default {
  components: {
    FormTask,
    //PDFViewer
  },
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    users: {
      type: Array,
      default: () => [],
    },
    nodes: {
      type: Array,
      default: () => [],
    },
    departments: {
      type: Array,
      default: () => [],
    },
    readonly: {
      type: Boolean,
      default: () => false,
    },
  },
  data() {
    return {
      blocks_approve: [],
      // thePdf: null,
      url: null,
      file: null,
      // sign: null,
    };
  },
  computed: {
    data_activity() {
      return store.data_activity;
    },
    current_user() {
      return store_auth.user;
    },
    listusersign() {
      var listusersign = this.model.data_setting.listusersign || [];
      for (var user of listusersign) {
        var user_id = user.user_sign;
        var findIndex = this.users.findLastIndex(function (item) {
          return item.id == user_id;
        });
        user.user = this.users[findIndex];
      }
      return listusersign;
    },
  },
  activated() {},
  mounted() {
    $(document).on("click", "#sign .signature.disabled", function (e) {
      var user_id = $(this).data("id");
      var sign = $("#pdf-viewer .signature[data-id='" + user_id + "']");
      $("#pdf-viewer").animate(
        {
          scrollTop: sign.parent()[0].offsetTop + sign[0].offsetTop - 20,
        },
        "slow"
      );
    });
    var model = this.model;
    var that = this;
    var indexBlock = this.nodes.findIndex(function (item) {
      return item.id == model.block_id;
    });
    var block = this.nodes[indexBlock];
    var blocks_approve_id = block.data_setting.blocks_approve_id || [];
    var blocks_approve = blocks_approve_id.map(function (block_id) {
      let indexActivity = that.data_activity.findLastIndex(function (i) {
        return i.block_id == block_id;
      });
      let activity = that.data_activity[indexActivity];
      return activity;
    });
    this.blocks_approve = blocks_approve;
    if (blocks_approve.length) this.activeName = blocks_approve[0].id;
    //console.log(blocks_approve);

    var blocks_esign_id = block.data_setting.blocks_esign_id;
    var indexActivity = this.data_activity.findLastIndex(function (item) {
      return item.block_id == blocks_esign_id;
    });
    if (indexActivity != -1) {
      var activity_esign = this.data_activity[indexActivity];
      var esign = activity_esign.data_setting.esign || {};
      //console.log(esign);
      var files = esign.files || [];
      var url = files[files.length - 1].url;
      this.file = files[files.length - 1].url;
      this.url = url;

      if (!that.readonly) {
        var pdfjsLib = window["pdfjs-dist/build/pdf"];
        // The workerSrc property shall be specified.
        pdfjsLib.GlobalWorkerOptions.workerSrc =
          "/src/assets/lib/pdfview/pdf.worker.js";

        // Asynchronous download of PDF
        var loadingTask = pdfjsLib.getDocument(this.url);
        loadingTask.promise.then(
          async function (pdf) {
            // await that.getsign();
            thePdf = pdf;

            var viewer = document.getElementById("pdf-viewer");
            for (var page = 1; page <= pdf.numPages; page++) {
              var canvas = document.createElement("canvas");
              canvas.className = "pdf-page-canvas";
              var div = document.createElement("div");
              div.className = "box-canvas";
              div.appendChild(canvas);
              viewer.appendChild(div);
              await that.renderPage(page, canvas);
            }
            ///FIND SUGGEST
            var indexActivity = that.data_activity.findLastIndex(function (
              item
            ) {
              return (
                item.data_setting.blocks_esign_id == blocks_esign_id &&
                item.clazz == "suggestTask"
              );
            });
            var activity_suggest = that.data_activity[indexActivity];
            var suggest =
              activity_suggest.data_setting.suggests[model.block_id];
            //console.log(suggest);
            if (suggest) {
              let parent = $(".box-canvas:eq(" + (suggest.page - 1) + ")");
              var sign = $("#sign .signature").clone();
              parent.append(sign);
              var height_page = $(".pdf-page-canvas", parent).height();
              $("#pdf-viewer .signature")
                .css({
                  left: suggest.position_x + "px",
                  top:
                    height_page -
                    (suggest.position_y + suggest.image_size_height + 40) +
                    "px",
                })
                .draggable({
                  stop: function (event, ui) {
                    if (ui.position.left < 0) {
                      var user_id = $(ui.helper).data("id");
                      $(
                        "#sign .disabled.signature[data-id='" + user_id + "']"
                      ).remove();
                      $(ui.helper)
                        .css({
                          top: 0 + "px",
                          left: 0 + "px",
                        })
                        .appendTo($("#sign"));
                    }
                  },
                });
              $("#pdf-viewer .signature .sign_image").resizable();
              $("#sign .signature").addClass("disabled");
            }
            ////
            // Fetch the first page
            $("#pdf-viewer").data("activity_esign", activity_esign.id);
            $("#sign .signature")
              .not(".disabled")
              .draggable({
                stop: function (event, ui) {
                  if (ui.position.left < 0) {
                    var user_id = $(ui.helper).data("id");
                    $(
                      "#sign .disabled.signature[data-id='" + user_id + "']"
                    ).remove();
                    $(ui.helper)
                      .css({
                        top: 0 + "px",
                        left: 0 + "px",
                      })
                      .appendTo($("#sign"));
                  }
                },
              });

            $("#sign .sign_image").not(".disabled").resizable();
            $(".box-canvas").droppable({
              // only accept elements matching this CSS selector
              accept: ".signature",
              // Require a 100% element overlap for a drop to be possible
              drop: function (event, ui) {
                var user_id = $(ui.draggable).data("id");
                $(ui.draggable).appendTo($(this));
                var newPosX = ui.offset.left - $(this).offset().left;
                var newPosY =
                  ui.offset.top - $(this).offset().top + this.scrollTop;
                $(ui.draggable).css({
                  top: newPosY + "px",
                  left: newPosX + "px",
                });
                //console.log(newPosX)
                if (newPosX < 0) {
                  $(ui.draggable)
                    .css({
                      top: 0 + "px",
                      left: 0 + "px",
                    })
                    .appendTo($("#sign"));
                } else {
                  var sign_in_init = $(
                    "#sign .signature[data-id='" + user_id + "']"
                  );
                  if (sign_in_init.length) {
                    // có
                    sign_in_init.addClass("disabled");
                  } else {
                    sign_in_init = $(ui.draggable).clone();
                    sign_in_init.addClass("disabled").css({
                      top: 0 + "px",
                      left: 0 + "px",
                    });
                    sign_in_init.appendTo($("#sign"));
                  }
                }
              },
            });
          },
          function (reason) {
            // PDF loading error
            console.error(reason);
          }
        );
      }
      setTimeout(function () {
        if (that.readonly) {
          $("#viewer").addClass("active");
          $("[href='#viewer']").addClass("active");
        } else {
          $("#approve-sign").addClass("active");
          $("[href='#approve-sign']").addClass("active");
        }
      }, 100);
    } else {
      setTimeout(function () {
        $("#tab-0").addClass("active");
        $("[href='#tab-0']").addClass("active");
      }, 100);
    }
  },
  methods: {
    require_sign(activity) {
      //console.log(activity)
      this.$emit("require_sign", activity);
    },
    async renderPage(pageNumber, canvas) {
      return thePdf.getPage(pageNumber).then(function (page) {
        var viewport = page.getViewport({
          scale: 1,
        });
        canvas.height = viewport.height;
        canvas.width = viewport.width;
        page.render({
          canvasContext: canvas.getContext("2d"),
          viewport: viewport,
        });
      });
    },
    // async getsign() {
    //   if (!this.current_user.is_sign) {
    //     alert("Bạn chưa có chữ ký điện tử. Vui lòng liên hệ Phòng IT!");
    //     return;
    //   }
    //   this.sign = sign;
    // },
  },
};
</script>
<style lang="scss" scoped>
#approve {
  height: 100%;
  overflow: auto;
}

.sign_image {
  background: transparent;
}

.activity {
  border-left: none;
}

.acti {
  i {
    flex: 0 0 36px;
    margin-right: 10px;
    /* width: 44px; */
    /* height: 36px; */
    text-align: center;
    line-height: 36px;
    border-radius: 12%;
    left: -19px;
    color: #0656ff;
    background-color: #f3f6f7;
    font-size: 20px;
    /* margin-top: -10px; */
    -webkit-box-shadow: 0 0 0 0.5px #f3f6f7;
    box-shadow: 0 0 0 0.5px #f3f6f7;
    -webkit-transform: rotate(45deg);
    transform: rotate(45deg);

    &.icon-warning {
      color: #fff;
      background-color: #ffb822;
    }

    &.icon-success {
      color: #fff;
      background-color: #1ecab8;
    }
  }
}
</style>
