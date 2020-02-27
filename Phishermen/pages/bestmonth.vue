<template>
  <div>
    <v-card color="red lighten-2" dark>
      <v-card-title class="headline red lighten-3">
        Best Month to fish in an area
      </v-card-title>
      <v-card-text>
        Search an area to find out the best month to fish
      </v-card-text>
      <v-card-text>
        <SearchBox
          :model="area"
          label="Area"
          url="GetAreas"
          item-value="areaNumber"
          item-text="areaName"
          :is-obj="true"
          placeholder="Search Area"
          @area="getArea"
        />
      </v-card-text>
      <v-card-actions>
        <v-btn @click="submit">
          Submit
        </v-btn>
      </v-card-actions>
    </v-card>
    <ColumnChart v-if="isSubmit" :data="data" :xaxis="xaxis" />
  </div>
</template>

<script>
import SearchBox from '../components/SearchBox'
import ColumnChart from '../components/ColumnChart'
export default {
  components: { ColumnChart, SearchBox },
  data: () => ({
    area: null,
    system: null,
    data: [],
    xaxis: [],
    isSubmit: false
  }),
  methods: {
    getArea(area) {
      this.area = area
    },
    getSystem(system) {
      this.system = system
    },
    submit() {
      this.data = []
      this.xaxis = []
      this.$axios
        .$get(`BestMonthInArea?areaNumber=${this.area.areaNumber}`)
        .then((res) => {
          for (let i = 0; i < res.length; i++) {
            this.data.push(res[i].fishCaught)
            this.xaxis.push(res[i].month)
          }
        })
        .then(() => {
          this.isSubmit = true
        })
    }
  }
}
</script>
