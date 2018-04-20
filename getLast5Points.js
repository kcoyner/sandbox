const axios = require('axios')

let dt = new Date('2018-04-18T05:23:30.000Z')

let dtE = dt.getTime()

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {'Authorization': 'pT4kQ4VifoHxQp6m27cbAbitEhs4Cs0zlE1KtV9Z9iv2r2871uT0QVarG1D991DC'},
  params: {
    filter: {
      where: {
        created: {
          gt: dtE
        }
      },
      order: 'created ASC',
      limit: 5
    }
  }
})

instance.get('/devices/463/points').then((resp) => {
  console.log('resp.data: ', resp.data)
})
