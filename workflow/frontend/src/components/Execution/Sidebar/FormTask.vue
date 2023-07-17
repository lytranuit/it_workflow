<template>
  <div>
    <div
      class="item-control flex-m"
      v-for="(element, index) in fields"
      :key="element.id"
    >
      <div class="mb-3 ml-3 flex-m w-100">
        <div class="form-input-data-preview mr-3 w-100">
          <div class="mb-2 form-input-control flex-m">
            <div class="custom-title-inline" style="width: 200px">
              <div class="container-label">
                <div class="container-left">
                  <div
                    id="idControlName"
                    class="font-weight-bold font-14 pr-2 d-inline-block"
                  >
                    <div style="">
                      {{ element.name }}
                      <span
                        class="text-danger"
                        style="float: inherit"
                        v-if="element.is_require"
                      >
                        *
                      </span>
                    </div>
                  </div>
                  <div
                    class="btn-selectTion-guide icon icon-infor-blue cursor-pointer icon-control"
                    style="color: #0c9cdd; float: right; margin-right: 5px"
                    v-show="element.guide && element.guide != ''"
                    :v-tooltip="element.guide"
                    placeholder="Bottom"
                  >
                    <i class="fas fa-info-circle" style="cursor: pointer"></i>
                  </div>
                </div>
              </div>
            </div>
            <div
              class="form-input-control-left w-100"
              v-if="element.type != 'table' && !readonly"
            >
              <div v-if="element.type == 'number'">
                <input
                  @change="signalChange(element.id)"
                  class="form-control form-control-sm number"
                  type="number"
                  v-model="element.values.value"
                  :required="element.is_require"
                  :name="element.id"
                />
              </div>
              <div v-if="element.type == 'formular'">
                <VueNumberFormat
                  class="form-control form-control-sm"
                  v-model="element.values.value"
                  :options="options_formular(element.data_setting)"
                  :name="element.id"
                  :required="element.is_require"
                  readonly
                ></VueNumberFormat>
              </div>
              <div v-if="element.type == 'currency'">
                <CurrencyInput
                  @change="signalChange(element.id)"
                  :name="element.id"
                  :required="element.is_require"
                  v-model="element.values.value"
                  :options="{
                    locale: 'de-DE',
                    currency: element.data_setting.currency || 'VND',
                    hideCurrencySymbolOnFocus: false,
                    hideGroupingSeparatorOnFocus: false,
                    hideNegligibleDecimalDigitsOnFocus: false,
                  }"
                />
              </div>

              <div v-if="element.type == 'text'">
                <input
                  class="form-control form-control-sm text"
                  type="text"
                  v-model="element.values.value"
                  :required="element.is_require"
                  :name="element.id"
                />
              </div>
              <div v-if="element.type == 'yesno'">
                <div class="custom-control custom-switch switch-success">
                  <input
                    type="checkbox"
                    class="custom-control-input"
                    :id="'customSwitch_' + element.id"
                    v-model="element.values.value"
                    :name="element.id"
                    value="1"
                  />
                  <label
                    class="custom-control-label"
                    :for="'customSwitch_' + element.id"
                  ></label>
                </div>
              </div>
              <div v-if="element.type == 'email'">
                <input
                  class="form-control form-control-sm email"
                  type="email"
                  v-model="element.values.value"
                  :required="element.is_require"
                  :name="element.id"
                />
              </div>
              <div v-if="element.type == 'file'">
                <input
                  class="form-control form-control-sm file"
                  type="file"
                  :required="element.is_require"
                  :name="element.id"
                />
              </div>
              <div v-if="element.type == 'file_multiple'">
                <input
                  class="form-control form-control-sm file"
                  type="file"
                  :required="element.is_require"
                  :name="element.id"
                  multiple
                />
              </div>
              <div v-if="element.type == 'date'">
                <Calendar
                  class="w-100"
                  v-model="element.values.value"
                  dateFormat="yy-mm-dd"
                  :required="element.is_require"
                  :name="element.id"
                />
              </div>
              <div v-if="element.type == 'date_month'">
                <Calendar
                  class="w-100"
                  v-model="element.values.value"
                  view="month"
                  dateFormat="yy-mm"
                  :required="element.is_require"
                  :name="element.id"
                />
              </div>

              <div v-if="element.type == 'date_time'">
                <Calendar
                  class="w-100"
                  id="calendar-24h"
                  v-model="element.values.value"
                  showTime
                  hourFormat="24"
                  :required="element.is_require"
                  :name="element.id"
                ></Calendar>
              </div>

              <div v-if="element.type == 'textarea'">
                <textarea
                  class="form-control form-control-sm textarea"
                  v-model="element.values.value"
                  :required="element.is_require"
                  :name="element.id"
                ></textarea>
              </div>

              <div v-if="element.type == 'select'">
                <TreeSelect
                  :options="get_options(element.data_setting.options)"
                  v-model="element.values.value"
                  :required="element.is_require"
                  name="tran"
                ></TreeSelect>
              </div>

              <div v-if="element.type == 'select_multiple'">
                <TreeSelect
                  :options="get_options(element.data_setting.options)"
                  v-model="element.values.value_array"
                  multiple
                  :required="element.is_require"
                  name="tran2"
                ></TreeSelect>
              </div>

              <div v-if="element.type == 'employee'">
                <TreeSelect
                  :options="users"
                  v-model="element.values.value"
                  :required="element.is_require"
                  :name="element.id"
                ></TreeSelect>
              </div>

              <div v-if="element.type == 'employee_multiple'">
                <TreeSelect
                  :options="users"
                  v-model="element.values.value_array"
                  multiple
                  :required="element.is_require"
                  :name="element.id"
                ></TreeSelect>
              </div>
              <div v-if="element.type == 'radio'">
                <div
                  class="radio radio-success radio-circle"
                  v-for="option in element.data_setting.options"
                  :key="option.id"
                >
                  <input
                    :id="'radio-' + option.id"
                    type="radio"
                    :name="'radio-' + element.id"
                    :value="option.id"
                    v-model="element.values.value"
                  />
                  <label :for="'radio-' + option.id">
                    {{ option.name }}
                  </label>
                </div>
              </div>

              <div v-if="element.type == 'checkbox'">
                <div
                  class="checkbox checkbox-success"
                  v-for="option in element.data_setting.options"
                  :key="option.id"
                >
                  <input
                    :id="'checkbox-' + option.id"
                    type="checkbox"
                    :value="option.id"
                    v-model="element.values.value_array"
                  />
                  <label :for="'checkbox-' + option.id">
                    {{ option.name }}
                  </label>
                </div>
              </div>
              <div v-if="element.type == 'department'">
                <TreeSelect
                  :options="departments"
                  v-model="element.values.value"
                  :required="element.is_require"
                  :name="element.id"
                ></TreeSelect>
              </div>

              <div v-if="element.type == 'department_multiple'">
                <TreeSelect
                  :options="departments"
                  v-model="element.values.value_array"
                  multiple
                  :required="element.is_require"
                  :name="element.id"
                ></TreeSelect>
              </div>
              <div v-if="element.type == 'task'">
                <div
                  class="checkbox checkbox-success checkbox-circle"
                  v-for="element in element.data_setting.options"
                  :key="element.id"
                >
                  <input :id="'task-' + element.id" type="checkbox" />
                  <label :for="'task-' + element.id">
                    {{ element.name }}
                  </label>
                </div>
              </div>
            </div>
            <div class="w-100" v-else v-html="display(element)"></div>
          </div>
          <div v-if="element.type == 'table'">
            <table
              class="table table-bordered mb-0 bg-white"
              style="outline: 1px solid #dee2e6 !important"
            >
              <thead class="">
                <tr>
                  <th
                    class="border-top-0 text-center"
                    v-for="(column, index1) in element.data_setting.columns"
                    :key="column.id"
                  >
                    {{ column.name }}
                    <span class="text-danger" v-if="column.is_require">*</span>
                  </th>
                  <th
                    v-if="!readonly && element.values.list_data.length > 1"
                  ></th>
                </tr>
              </thead>
              <tbody v-if="!readonly">
                <tr
                  v-for="(row, index1) in element.values.list_data"
                  :key="index1"
                >
                  <td
                    v-for="(column, index2) in element.data_setting.columns"
                    :key="column.id"
                    class="text-center"
                  >
                    <div v-if="column.type == 'stt'">
                      {{ row[column.id] }}
                    </div>
                    <div v-if="column.type == 'formular'">
                      <VueNumberFormat
                        class="form-control form-control-sm"
                        v-model="row[column.id]"
                        :options="options_formular(column)"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                        readonly
                      ></VueNumberFormat>
                    </div>
                    <div v-if="column.type == 'number'">
                      <input
                        class="form-control form-control-sm number"
                        type="number"
                        v-model="row[column.id]"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                        @change="
                          signalChange(
                            column.id,
                            index1,
                            element.data_setting.columns,
                            element.values.list_data
                          )
                        "
                      />
                    </div>
                    <div v-if="column.type == 'currency'">
                      <CurrencyInput
                        @change="
                          signalChange(
                            column.id,
                            index1,
                            element.data_setting.columns,
                            element.values.list_data
                          )
                        "
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                        v-model="row[column.id]"
                        :options="{
                          locale: 'de-DE',
                          currency: column.currency || 'VND',
                          hideCurrencySymbolOnFocus: false,
                          hideGroupingSeparatorOnFocus: false,
                          hideNegligibleDecimalDigitsOnFocus: false,
                        }"
                      />
                    </div>
                    <div v-if="column.type == 'text'">
                      <input
                        class="form-control form-control-sm text"
                        type="text"
                        v-model="row[column.id]"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                      />
                    </div>
                    <div v-if="column.type == 'yesno'">
                      <div class="custom-control custom-switch switch-success">
                        <input
                          type="checkbox"
                          class="custom-control-input"
                          :id="'customSwitch_' + column.id + '_' + index1"
                          v-model="row[column.id]"
                          :name="column.id + '_' + index1"
                          value="1"
                        />
                        <label
                          class="custom-control-label"
                          :for="'customSwitch_' + column.id + '_' + index1"
                        ></label>
                      </div>
                    </div>
                    <div v-if="column.type == 'email'">
                      <input
                        class="form-control form-control-sm email"
                        type="email"
                        v-model="row[column.id]"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                      />
                    </div>
                    <div v-if="column.type == 'date'">
                      <input
                        class="form-control form-control-sm date"
                        type="text"
                        v-model="row[column.id]"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                      />
                    </div>
                    <div v-if="column.type == 'date_month'">
                      <input
                        class="form-control form-control-sm date_month"
                        type="text"
                        v-model="row[column.id]"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                      />
                    </div>

                    <div v-if="column.type == 'date_time'">
                      <input
                        class="form-control form-control-sm date_time"
                        type="text"
                        v-model="row[column.id]"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                      />
                    </div>
                    <div v-if="column.type == 'textarea'">
                      <textarea
                        class="form-control form-control-sm textarea"
                        v-model="row[column.id]"
                        :name="column.id + '_' + index1"
                        :required="column.is_require"
                      ></textarea>
                    </div>
                  </td>
                  <td v-if="element.values.list_data.length > 1">
                    <div
                      class="ml-2 text-danger"
                      style="cursor: pointer"
                      @click="remove_row(element, index1)"
                    >
                      <i class="fas fa-trash-alt"></i>
                    </div>
                  </td>
                </tr>
                <tr>
                  <td :colspan="element.data_setting.columns.length">
                    <a
                      href="#"
                      style="color: #1e9dc3"
                      @click="add_row_table(element)"
                      ref="addRowTable"
                      :data-count="element.values.list_data.length"
                      ><i class="fas fa-plus"></i> Thêm dòng</a
                    >
                  </td>
                </tr>
              </tbody>
              <tbody v-else>
                <tr
                  v-for="(row, index1) in element.values.list_data"
                  :key="index1"
                >
                  <td
                    v-for="(column, index2) in element.data_setting.columns"
                    :key="column.id"
                    class="text-center"
                  >
                    <div v-if="column.type == 'currency'">
                      {{ format_currency(row[column.id], column.currency) }}
                    </div>
                    <div v-else-if="column.type == 'formular'">
                      {{
                        format_formular(
                          row[column.id],
                          options_formular(column)
                        )
                      }}
                    </div>
                    <div v-else-if="column.type == 'yesno'">
                      <i
                        v-if="row[column.id] == 'true'"
                        class="far fa-check-circle text-success"
                      ></i>
                      <i
                        v-if="row[column.id] != 'true'"
                        class="fas fa-ban text-danger"
                      ></i>
                    </div>
                    <div v-else>{{ row[column.id] }}</div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
// import mitt from "mitt";

import stringMath from "string-math";
// const emitter = mitt();
import CurrencyInput from "../../CurrencyInput.vue";
import VueNumberFormat from "@igortrindade/vue-number-format";
import Calendar from "primevue/calendar";
import { rand } from "../../../utilities/rand";
export default {
  components: {
    CurrencyInput,
    VueNumberFormat,
  },
  props: {
    fields: {
      type: Array,
      default: () => [],
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
  mounted() {
    var index = 0;
    $(".vue-TreeSelect__input").each(function () {
      $(this).attr("name", "index_" + index++);
    });
    if (this.$refs.addRowTable) {
      this.$refs.addRowTable.map(function (item) {
        if ($(item).data("count") == 0) {
          item.click();
        }
      });
    }
    var firstvariable = "!#"; //first input;
    var secondvariable = "#"; //first in
    var fields = this.fields;
    if (fields) {
      for (var field of fields) {
        if (field.type == "table" && field.data_setting.columns) {
          var columns = field.data_setting.columns;
          for (var column of columns) {
            if (column.type == "formular") {
              var text = column.formular.text;
              var list_id = text.match(
                new RegExp(
                  "(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")",
                  "g"
                )
              );
              //console.log(text);
              //console.log(list_id);
              //   for (var id of list_id) {
              //     emitter.on(id, (e) => {
              //       console.log(e);
              //     });
              //   }
            }
          }
        }
      }
    }
  },
  methods: {
    options_formular(column) {
      var suffix = "";
      if (column.formular.type_return == "percent") {
        suffix = " %";
      } else if (column.formular.type_return == "currency") {
        suffix = " VND";
      }
      return {
        precision: column.formular.decimal_number,
        prefix: "",
        suffix: suffix,
        decimal: ",",
        thousand: ".",
        acceptNegative: false,
        isInteger: false,
      };
    },
    signalChange: function (id, index = null, columns = [], list_data = []) {
      //console.log(evt.target);
      //var name = $(evt.target).attr("name");
      //var list = name.split("_");
      //var id = list[0];
      //var index = list[1];
      var firstvariable = "!#"; //first input;
      var secondvariable = "#"; //first in
      if (index != null) {
        //// CHANGE IN TABLE
        var row = list_data[index];
        for (var column of columns) {
          if (column.type == "formular") {
            var column_id = column.id;
            var text = column.formular.text;
            var list_id = text.match(
              new RegExp(
                "(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")",
                "g"
              )
            );
            //console.log(list_id)
            //console.log(text)
            if (list_id.indexOf(id) == -1) {
              continue;
            }
            for (var _id of list_id) {
              var value = row[_id] ? row[_id] : 0;
              text = text.replace(new RegExp("!#" + _id + "#", "g"), value);
            }
            var result = stringMath(text, function (err) {
              return 0;
            });
            row[column_id] = result;
            this.signalChange(column_id, index, columns, list_data);
            //console.log(text);
            //console.log(row[column_id]);
          }
        }
      }
      //// CHANGE IN FIELD
      var field_formular = this.fields.filter(function (item) {
        return item.type == "formular";
      });
      if (field_formular) {
        for (var field of field_formular) {
          if (
            field.data_setting.formular.type == 2 &&
            id == field.data_setting.formular.operator_column
          ) {
            var operator_type = field.data_setting.formular.operator_type;
            var arr = list_data.map(function (item) {
              return item[id];
            });
            switch (operator_type) {
              case "sum":
                field.values.value = arr.reduce((a, b) => a + b, 0);
                //console.log(arr);
                break;
              case "avg":
                field.values.value =
                  arr.length > 0
                    ? arr.reduce((a, b) => a + b, 0) / arr.length
                    : 0;
                break;
              case "min":
                field.values.value = Math.min(...arr);
                break;
              case "max":
                field.values.value = Math.max(...arr);
                break;
            }
          }
          if (field.data_setting.formular.type == 1) {
            var text = field.data_setting.formular.text;
            var list_id = text.match(
              new RegExp(
                "(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")",
                "g"
              )
            );
            if (list_id.indexOf(id) == -1) {
              continue;
            }
            for (var _id of list_id) {
              var findindex = this.fields.findIndex(function (item) {
                return item.id == _id;
              });
              var findField = this.fields[findindex];
              var value = findField.values.value ? findField.values.value : 0;
              text = text.replace(new RegExp("!#" + _id + "#", "g"), value);
            }
            var result = stringMath(text, function (err) {
              return 0;
            });
            field.values.value = result;
          }
        }
      }
      this.$forceUpdate();
    },

    get_options(options) {
      return options.map(function (item) {
        item.label = item.name;
        return item;
      });
    },
    add_row_table(field) {
      var columns = field.data_setting.columns;
      var data = {};
      var column_stt = null;
      for (var column of columns) {
        if (column.type == "stt") {
          column_stt = column.id;
        } else {
          data[column.id] = null;
        }
      }
      field.values.list_data.push(data);
      if (column_stt) {
        field.values.list_data = field.values.list_data.map(function (
          item,
          key
        ) {
          item[column_stt] = key + 1;
          return item;
        });
      }
      var name = field.name;
      field.name = rand();
      field.name = name;
    },
    remove_row(element, index) {
      element.values.list_data.splice(index, 1);

      var columns = element.data_setting.columns;
      var column_stt = null;
      for (var column of columns) {
        if (column.type == "stt") {
          column_stt = column.id;
        }
      }
      if (column_stt) {
        element.values.list_data = element.values.list_data.map(function (
          item,
          key
        ) {
          item[column_stt] = key + 1;
          return item;
        });
      }
      var name = element.name;
      element.name = rand();
      element.name = name;
    },
    display(field) {
      var text = field.values.value;
      var data_setting = field.data_setting;
      if (field.type == "select" || field.type == "radio") {
        var options = data_setting.options;
        var index = options.findIndex(function (item) {
          return item.id == field.values.value;
        });
        if (index != -1) {
          var option = options[index];
          text = option.name;
        }
      } else if (field.type == "file" || field.type == "file_multiple") {
        text = "";
        if (field.values.files) {
          for (var file of field.values.files) {
            text +=
              `
                                <div class="flex-m mb-1">
                                    <div class="file-icon" data-type="` +
              file.ext +
              `"></div>
                                    <a href="` +
              file.url +
              `" download="` +
              file.name +
              `" style="margin-left: 5px;">
							            ` +
              file.name +
              `
						            </a>
                                </div>`;
          }
        }
      } else if (field.type == "department") {
        var index = this.departments.findIndex(function (item) {
          return item.id == field.values.value;
        });
        if (index != -1) {
          var option = this.departments[index];
          text = option.label;
        }
      } else if (field.type == "employee") {
        var index = this.users.findIndex(function (item) {
          return item.id == field.values.value;
        });
        if (index != -1) {
          var option = this.users[index];
          text = option.label;
        }
      } else if (field.type == "select_multiple" || field.type == "checkbox") {
        var value_array = field.values.value_array || [];
        var options = data_setting.options;
        var list = options
          .filter(function (item) {
            return value_array.indexOf(item.id) != -1;
          })
          .map(function (item) {
            return item.name;
          });
        text = list.join(", ");
      } else if (field.type == "select_department") {
        var value_array = field.values.value_array || [];
        var list = this.departments
          .filter(function (item) {
            return value_array.indexOf(item.id) != -1;
          })
          .map(function (item) {
            return item.label;
          });
        text = list.join(", ");
      } else if (field.type == "select_employee") {
        var value_array = field.values.value_array || [];
        var list = this.users
          .filter(function (item) {
            return value_array.indexOf(item.id) != -1;
          })
          .map(function (item) {
            return item.label;
          });
        text = list.join(", ");
      } else if (field.type == "date") {
        text = field.values.value
          ? moment(field.values.value).format("YYYY-MM-DD")
          : "";
      } else if (field.type == "date_month") {
        text = field.values.value
          ? moment(field.values.value).format("YYYY-MM")
          : "";
      } else if (field.type == "date_time") {
        text = field.values.value
          ? moment(field.values.value).format("YYYY-MM-DD HH:mm")
          : "";
      } else if (field.type == "currency") {
        text = field.values.value
          ? new Intl.NumberFormat("de-DE", {
              style: "currency",
              currency: data_setting.currency,
            }).format(field.values.value)
          : "";
      } else if (field.type == "formular") {
        text = field.values.value
          ? VueNumberFormat.format(
              field.values.value,
              this.options_formular(field.data_setting)
            )
          : "";
      } else if (field.type == "yesno") {
        //console.log(field.values.value);
        text =
          field.values.value && field.values.value == "true"
            ? "<span class='text-success'><i class='far fa-check-circle'></i> Chọn</span>"
            : "<span class='text-danger'><i class='fas fa-ban'></i> Không chọn</span>";
      }
      return text;
    },
    format_currency(value, currency) {
      return new Intl.NumberFormat("de-DE", {
        style: "currency",
        currency: currency,
      }).format(value);
    },

    format_formular(value, option) {
      return VueNumberFormat.format(value, option);
    },
  },
};
</script>
<style lang="scss" scoped></style>
