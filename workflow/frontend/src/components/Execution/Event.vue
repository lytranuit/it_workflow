<template>
  <div class="card no-shadow border">
    <div class="card-header">
      <b>Sự kiện</b>
    </div>
    <div class="card-body" style="max-height: 500px; overflow: auto">
      <div class="activity" id="event">
        <div v-for="(event, index) in events">
          <i
            class="mdi"
            :class="{
              'mdi-checkbox-marked-circle-outline icon-success':
                event.type != 2,
              'mdi-close-circle icon-danger': event.type == 2,
            }"
          ></i>
          <div class="time-item">
            <div class="item-info">
              <div class="d-flex justify-content-between align-items-center">
                <span class="">{{
                  formatDate(event.created_at, "HH:mm DD/MM/YYYY")
                }}</span>
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
      events: [],
    };
  },
  mounted() {
    this.getEvents();
  },
  methods: {
    async getEvents() {
      /// Lấy event
      var execution_id = this.model.id;
      var ress = await Api.events(execution_id);
      var events = ress.events;
      this.events = events;
    },
    formatDate: formatDate,
  },
};
</script>

<style lang="scss"></style>
