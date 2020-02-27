<template>
  <div id="chart">
    <apexchart
      type="bar"
      height="350"
      :options="chartOptions"
      :series="series"
    ></apexchart>
  </div>
</template>

<script>
export default {
  name: 'StackColumnChart',
  props: {
    data: {
      default() {
        return ['100']
      },
      type: Array
    }
  },
  data() {
    return {
      chartOptions: {
        chart: {
          type: 'bar',
          height: 350,
          stacked: true,
          stackType: '100%'
        },
        responsive: [
          {
            breakpoint: 480,
            options: {
              legend: {
                position: 'bottom',
                offsetX: -10,
                offsetY: 0
              }
            }
          }
        ],
        xaxis: {
          categories: [
            'Jan',
            'Feb',
            'Mar',
            'Apr',
            'May',
            'Jun',
            'Jul',
            'Aug',
            'Sep',
            'Oct',
            'Nov',
            'Dec'
          ]
        },
        fill: {
          opacity: 1
        },
        legend: {
          position: 'right',
          offsetX: 0,
          offsetY: 50
        },
        tooltip: {
          y: {
            formatter(val) {
              return val + ' fishes'
            }
          }
        }
      }
    }
  },
  computed: {
    series() {
      return this.data
    }
  }
}
</script>

<style scoped></style>
