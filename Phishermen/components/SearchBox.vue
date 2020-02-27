<template>
  <v-autocomplete
    v-model="model"
    :items="items"
    :loading="isLoading"
    :search-input.sync="search"
    hide-no-data
    hide-selected
    filled
    :item-text="itemText"
    :item-value="itemValue"
    :chips="isMultiple"
    :multiple="isMultiple"
    :label="label"
    :placeholder="placeholder"
    prepend-icon="mdi-database-search"
    return-object
    clearable
    @input="input"
  >
    <template v-if="isMultiple" v-slot:selection="data">
      <v-chip
        v-bind="data.attrs"
        :input-value="data.selected"
        close
        @click="data.select"
        @click:close="remove(data.item)"
      >
        {{ data.item[itemText] || data.item }}
      </v-chip>
    </template>
  </v-autocomplete>
</template>

<script>
export default {
  name: 'SearchBox',
  props: {
    itemText: {
      default: '',
      type: String
    },
    itemValue: {
      default: '',
      type: String
    },
    label: {
      default: '',
      type: String
    },
    placeholder: {
      default: '',
      type: String
    },
    url: {
      default: '',
      type: String
    },
    isMultiple: {
      default: false,
      type: Boolean
    },
    isObj: {
      default: false,
      type: Boolean
    }
  },
  data: () => ({
    descriptionLimit: 10,
    entries: [],
    isLoading: false,
    model: null,
    search: null
  }),
  computed: {
    items() {
      if (this.isMultiple || this.isObj) {
        return this.entries
      } else {
        return this.entries.map((entry) => {
          return String(entry)
        })
      }
    }
  },

  watch: {
    search(val) {
      // Items have already been loaded
      if (this.items.length > 0) return

      // Items have already been requested
      if (this.isLoading) return

      this.isLoading = true

      // Lazily load input items
      this.$axios
        .$get(this.url)
        .then((res) => {
          // console.log(res)
          this.entries = res
        })
        .catch((err) => {
          // eslint-disable-next-line no-console
          console.log(err)
        })
        .finally(() => (this.isLoading = false))
    }
  },
  methods: {
    input() {
      this.$emit(this.label.toLowerCase(), this.model)
    },
    remove(item) {
      const index = this.model.findIndex((a) => a === item)
      if (index >= 0) this.model.splice(index, 1)
    }
  }
}
</script>

<style scoped></style>
