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
          :is-multiple="true"
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
    <!--    <ColumnChart v-if="isSubmit" :data="data" :xaxis="xaxis" />-->
    <MulColumnChart v-if="isSubmit" :data="data" />
    <StackColumnChart v-if="isSubmit" :data="data" />
    <MulRadarChart v-if="isSubmit" :data="data" />
  </div>
</template>

<script>
import SearchBox from '../components/SearchBox'
import MulColumnChart from '../components/MulColumnChart'
import StackColumnChart from '../components/StackColumnChart'
import MulRadarChart from '../components/MulRadarChart'
export default {
  components: { MulRadarChart, StackColumnChart, MulColumnChart, SearchBox },
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
    async submit() {
      this.isSubmit = false
      this.data = []
      // this.xaxis = []
      // console.log(this.area)
      for (let i = 0; i < this.area.length; i++) {
        const r = await this.$axios.$get(
          `BestMonthInArea?areaNumber=${this.area[i].areaNumber}`
        )
        this.data[i] = {
          name: this.area[i].areaName,
          data: []
        }
        for (let j = 0; j < r.length; j++) {
          this.data[i].data.push(r[j].fishCaught)
        }
      }
      this.isSubmit = true
      console.log(this.data)
      // this.$axios
      //   .$get(`BestMonthInArea?areaNumber=${this.area.areaNumber}`)
      //   .then((res) => {
      //     for (let i = 0; i < res.length; i++) {
      //       this.data.push(res[i].fishCaught)
      //       this.xaxis.push(res[i].month)
      //     }
      //   })
      //   .then(() => {
      //     this.isSubmit = true
      //   })
    }
  }
}
</script>
