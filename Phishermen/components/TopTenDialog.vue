<template>
  <v-row justify="center">
    <v-dialog v-model="dialog" persistent fullscreen>
      <v-card>
        <v-card-title class="headline">
          Top Ten Area in {{ regionName }}
        </v-card-title>
        <v-card-text>
          <ColumnChart :data="data" :xaxis="xaxis" />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="green darken-1" text @click="close">
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import ColumnChart from './ColumnChart'
export default {
  name: 'TopTenDialog',
  components: { ColumnChart },
  props: {
    regionName: {
      type: String,
      default: ''
    },
    index: {
      type: Number,
      default: 1
    }
  },
  data: () => ({
    dialog: false,
    isSubmit: false,
    data: [],
    xaxis: []
  }),
  created () {
    this.submit()
  },
  methods: {
    close () {
      this.dialog = false
      this.$emit('setIsDialog', this.index)
    },
    async submit () {
      this.dialog = true
      this.data = []
      const r = await this.$axios.$get(
        `RegionTopTenAreas?regionName=${this.regionName}`
      )
      for (let i = 0; i < r.length; i++) {
        this.data.push(r[i].averageHaul)
        this.xaxis.push(r[i].areaName || 'null')
      }
    }
  }

}
</script>

<style scoped>

</style>
