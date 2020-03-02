<template>
  <div>
    <v-row>
      <span style="margin: auto;">
        <h1>WELCOME FISHERMEN</h1>
      </span>
    </v-row>
    <br>
    <v-carousel
      cycle
      interval="5000"
      show-arrows-on-hover
    >
      <v-carousel-item
        v-for="(item, i) in items"
        :key="i"
        reverse-transition="fade-transition"
        :src="item.src"
      >
        <h2 style="color:white; position: absolute;left: 0;top: 30%; width: 100%; padding-left: 100px; padding-right:100px; text-align: center;">
          {{ messages[i] }}
        </h2>
        <!-- <h3 style="margin-top:-100px">
              <a href="/bestplace">{{ messages[i] }}</a>
        </h3> -->
      </v-carousel-item>
    </v-carousel>
  </div>
</template>

<script>
// import Yak from "../assets/YakRiver.jpg";
export default {
  data: () => ({
    images: [
      '~/assets/lakechelan.jpg',
      'assets/12_Fishing.jpg',
      'assets/Steelhead-Washington-state-fish.jpg',
      '~/assets/Fleet.jpg'
    ],
    items: [

      {
        src: '/YakRiver.jpg'
      },
      {
        src: '/12_Fishing.jpg'
      },
      {
        src: '/Steelhead-Washington-state-fish.jpg'
      },
      {
        src: '/Fleet.jpg'
      }
      // {
      //   src: "assets/12_Fishing.jpg"
      // },
      // {
      //   src: "assets/Steelhead-Washington-state-fish.jpg"
      // },
      // {
      //   src: "~/assets/Fleet.jpg"
      // }
    ],
    messages: [
      'Fish for the Best Location!',
      'Fish for the Best Season!',
      'The Fishermen website is the Best tool to find the Best Fishing!',
      'All in Washington State!'
    ]
  })
}
</script>
<!--'The Fishermen website is a tool to search any fishing location in Washington State and by any month to find the best place to go fishing with your fellow fishermen!',-->

<!--//'The Fishermen website is a tool to analyze the best place to fish, and the best time to fish. Search any fishing location in Washington State and you will find the best time to fish in that spot. Or you can seach by month to find the best place to go fishing for that particular time. Finding the best time and place to fish will give you an advantage over your fellow fishermen!',-->
