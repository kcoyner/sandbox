var axios = require('axios')

// var instance = axios.create({
//   baseURL: 'https://cp.remotethings.co.uk/api',
//   timeout: 5000,
//   headers: {'Authorization': 'pT4kQ4VifoHxQp6m27cbAbitEhs4Cs0zlE1KtV9Z9iv2r2871uT0QVarG1D991DC'},
// })
// instance.get('/devices/463/points').then((resp) => {
//   console.log('resp.data: ', resp)
//   console.log('resp.data: ', resp.data[10].location)
// })

let start = new Date('2018-03-21T05:20:30.000Z')
let end = new Date('2018-03-21T05:25:30.000Z')

let startE = start.getTime()
let endE = end.getTime()

var config = {
  url: 'https://cp.remotethings.co.uk/api/devices/463/points',
  headers: {
    Authorization:
      'pT4kQ4VifoHxQp6m27cbAbitEhs4Cs0zlE1KtV9Z9iv2r2871uT0QVarG1D991DC'
  },
  params: {
    // filter: {where:{timestamp: {gt: 1510332638000 }}}
    filter: {
      where: {
        timestamp: {
          between: [startE, endE]
        }
      }
    }
  }
}

axios
  .request(config)
  .then(resp => {
    console.log('resp.data: ', resp.data)
  })
  .catch(error => {
    console.log(error)
  })
