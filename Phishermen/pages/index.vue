<style>
  .modal {
    margin: auto;
    background-color: rgba(255, 255, 255, 0.40);
    border-radius: 5px;
    color: #333;
    font-family: sans-serif;
    line-height: 1.5;
    margin-top:15%;
    width:auto;
    max-width: fit-content;
    padding: 1rem 2rem;
    backdrop-filter: blur(1px);
    background-color: rgba(0, 0, 0, 0.5);
    a {
      color: #bf0222;
    }
  }
  .content-container {
    align-items: center;
    justify-content: center;
    position: center;
    padding-left: 5px;
    padding-right: 5px;
    height: 100%;
    width: auto%;
  }
</style>
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
        style="align-content: center"
      >
        <!--        <div class="module">-->
        <!--          <header>-->
        <!--            <h1>-->
        <!--              Skyscraper-->
        <!--            </h1>-->
        <!--          </header>-->
        <!--        </div>-->
        <div class="content-container">
          <div class="modal">
            <div style="text-shadow: 0 3px black;position: relative;z-index: 1;background-size: 30px 30px ; color:white; position: relative;top: 30%; width: 100%; padding-left: 10px; padding-right:10px; text-align: center;">
              <h2 v-if="locationss[i] !== ''">
                <a :href="locationss[i]">
                  {{ messages[i] }}
                </a>
              </h2>
              <h2 v-else :href="locationss[i]">
                {{ messages[i] }}
              </h2>
            </div>
          </div>
        </div>
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
    locationss: [

      '/bestplace',
      '/bestmonth',
      '',
      ''
    ],
    locations: [

      {
        href: '/bestmonth'
      },
      {
        href: '/bestplace'
      },
      {
        href: ''
      },
      {
        href: ''
      }
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
