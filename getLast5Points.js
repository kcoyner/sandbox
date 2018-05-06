const axios = require('axios')

let dt = new Date('2018-04-18T05:23:30.000Z')

let dtE = dt.getTime()

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {'Authorization': 'PJeoXCVNAZoOI4K8f0o577IckQirhQmETUwqf85nfpo8Ndkc7n43mPDjSSlfuDsK'},
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
}).catch((error) => console.log(error))
