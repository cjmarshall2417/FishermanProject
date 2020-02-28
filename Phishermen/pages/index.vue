<template>
  <div>
    <v-row>
      <v-col> </v-col>
      <v-col>
        <h1>WELCOME FISHERMEN</h1>
      </v-col>
      <v-col> </v-col>
    </v-row>
    <v-carousel
      cycle
      height="400"
      hide-delimiter-background
      show-arrows-on-hover
    >
      <v-carousel-item>
        <v-sheet height="100%">
          <v-row class="fill-height" align="center" justify="center">
            <v-col> </v-col>
            <v-col>
              <img width="600" height="300" src="~/assets/YakRiver.jpg" />
            </v-col>
            <v-col>
              <h3 style="margin-top:-100px"><a href="/bestplace">Fish for the Best Location!</a></h3>
            </v-col>
          </v-row>
        </v-sheet>
      </v-carousel-item>
      <v-carousel-item>
        <v-sheet height="100%">
          <v-row>
            <v-col> </v-col>
            <v-col>
              <h3><a href="/bestmonth">Fish for the Best Season!</a></h3>
            </v-col>
            <v-col> </v-col>
          </v-row>
          <v-row class="fill-height" align="center" justify="center">
            <v-col> </v-col>
            <v-col>
              <img
                style="layout:auto; margin-top:-90px"
                src="~/assets/lakechelan.jpg"
              />
            </v-col>

            <v-col> </v-col>
          </v-row>
        </v-sheet>
      </v-carousel-item>
      <v-carousel-item>
        <v-sheet height="100%">
          <v-row class="fill-height" align="center" justify="center">
            <v-col>
            </v-col>
            <v-col>
              <img
                width="800"
                height="600"
                style="layout:auto; margin-top:-70px; margin-left:-200px"
                src="~/assets/12_Fishing.jpg"
                align="center"
              />
            </v-col>
            <v-col>
            <h4 style="width:100%;layout:auto;margin-top:-100px">
              The Fishermen website is a tool to analyze the best place to fish,
              and the best time to fish. Search any fishing location in
              Washington State and you will find the best time to fish in that
              spot. Or you can seach by month to find the best place to go
              fishing for that particular time. Finding the best time and place
              to fish will give you an advantage over your fellow fishermen!
            </h4>
            </v-col>
          </v-row>
        </v-sheet>
      </v-carousel-item>
      <v-carousel-item>
        <v-sheet height="100%">
          <v-row class="fill-height" align="center" justify="center">
            <img
              style="layout:auto; margin-top:-230px"
              src="~/assets/Steelhead-Washington-state-fish.jpg"
            />
          </v-row>
        </v-sheet>
      </v-carousel-item>
      <v-carousel-item>
        <v-sheet height="100%">
          <v-row class="fill-height" align="center" justify="center">
            <div class="container">
              <img
                style="layout:auto; margin-left:200px"
                src="~/assets/lakechelan.jpg"
                align="center"
              />
            </div>
          </v-row>
        </v-sheet>
      </v-carousel-item>
    </v-carousel>
  </div>
</template>

<script>
import SearchBox from "../components/SearchBox";
import ColumnChart from "../components/ColumnChart";
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
      this.area = area;
    },
    getSystem(system) {
      this.system = system;
    },
    submit() {
      this.data = [];
      this.xaxis = [];
      this.$axios
        .$get(`BestMonthInArea?areaNumber=${this.area.areaNumber}`)
        .then(res => {
          console.log(res);
          for (let i = 0; i < res.length; i++) {
            this.data.push(res[i].fishCaught);
            this.xaxis.push(res[i].month);
          }
        })
        .then(() => {
          this.isSubmit = true;
        });
      // console.log(this.area.areaNumber)
      // console.log(this.system)
    }
  }
};
</script>
