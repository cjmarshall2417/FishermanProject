<template>
  <div id="chart">
    <apexchart
      type="radar"
      height="350"
      :options="chartOptions"
      :series="series"
    />
  </div>
</template>

<script>
export default {
  name: 'MulRadarChart',
  props: {
    data: {
      default () {
        return ['100']
      },
      type: Array
    }
  },
  data () {
    return {
      chartOptions: {
        chart: {
          height: 350,
          type: 'radar',
          dropShadow: {
            enabled: true,
            blur: 1,
            left: 1,
            top: 1
          }
        },
        title: {
          text: 'Radar Chart - Multi Series'
        },
        stroke: {
          width: 0
        },
        fill: {
          opacity: 0.4
        },
        markers: {
          size: 0
        },
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
        }
      }
    }
  },
  computed: {
    series () {
      return this.data
    }
  }
}
</script>

<style scoped></style>
