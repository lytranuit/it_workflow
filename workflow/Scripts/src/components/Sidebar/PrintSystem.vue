<template>
    <div class="text-center container h-100">
        <ul class="nav nav-pills nav-justified" role="tablist">
            <li class="nav-item waves-effect waves-light">
                <a class="nav-link active" data-toggle="tab" href="#viewer" role="tab" aria-selected="true">Xem file</a>
            </li>
            <li class="nav-item waves-effect waves-light">
                <a class="nav-link" data-toggle="tab" href="#suggest" role="tab" aria-selected="false">Gợi ý vị trí ký</a>
            </li>
        </ul>
        <div class="tab-content mt-3" style="height: calc(100% - 20px)">
            <div class="tab-pane active h-100" id="viewer" role="tabpanel">
                <embed :src="url" style="width: 100%;height: 100%;" type="application/pdf">
            </div>
            <div class="tab-pane h-100" id="suggest" role="tabpanel">
                <div class="card no-shadow border">
                    <div class="card-body">
                        <div class="row g-0">
                            <div class="col-12">
                                <strong>Gợi ý vị trí ký</strong><br />
                                <small class="text-muted">(Kéo thả người ký từ khung bên trái sang văn bản ở khung bên phải để gợi ý vị trý ký)</small>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-9" style="border: 5px solid #d7d7d7;border-left: 0;">
                                <div id='pdf-viewer' class=""></div>
                            </div>
                            <div class="col-3 order-first" style="border: 5px solid #d7d7d7;height: 80vh;display: inline-block;padding:10px">
                                <div class="base-title">Chữ ký</div>
                                <span class="base-subtitle">Kéo chữ ký vào văn bản để ký</span>
                                <div id="sign">
                                </div>
                            </div>
                            <div class="col-md-12 text-center mt-3">
                                <button class="btn-primary btn savefile">Lưu gợi ý vị trí chữ ký</button>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        inject: ['i18n'],
        components: {
        },
        props: {
            model: {
                type: Object,
                default: () => ({}),
            },
            nodes: {
                type: Array,
                default: () => ([]),
            },
        },
        data() {
            return {

            }
        },
        computed: {
            url() {
                var esign = this.model.data_setting.esign || {};
                console.log(esign);
                var files = esign.files || [];
                return path + files[0].url;
            },
            signatures() {
                var esign = this.model.data_setting.esign || {};
                return esign.signatures;
            }

        },
        mounted() {


        },
        methods: {
        }
    }
</script>
<style lang="scss" scoped></style>
