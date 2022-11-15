<template>
    <div class="card no-shadow border">
        <div class="card-header">
            <b>Sự kiện</b>
        </div>
        <div class="card-body" style="max-height: 500px;overflow: auto;">
            <div class="activity" id="event">
                <div v-for="(event,index) in events">
                    <i class="mdi mdi-checkbox-marked-circle-outline icon-success"></i>
                    <div class="time-item">
                        <div class="item-info">
                            <div class="d-flex justify-content-between align-items-center">
                                <span class="">{{event.created_at | moment('HH:mm DD/MM/YYYY')}}</span>
                            </div>
                            <h5 v-html="event.event_content"></h5>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
export default {
        components: {
        },
        props: {
            model: {
                type: Object,
                default: () => ({}),
            },
        },
        data() {
            return {
                events: [],
                path: path
            };
        },
        mounted() {
            this.getEvents();
        },
        methods: {
            async getEvents() {
                /// Lấy event
                var execution_id = this.model.id;
                var ress = await $.ajax({
                    url: path + "/admin/api/events",
                    data: { execution_id: execution_id },
                });
                var events = ress.events;
                this.events = events;

            },
        }
}
</script>

<style lang="scss"></style>
