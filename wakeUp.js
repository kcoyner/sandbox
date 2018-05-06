const axios = require('axios')

var instance = axios.create({
  baseURL: 'https://cp.remotethings.co.uk/api',
  timeout: 5000,
  headers: {'Authorization': 'PJeoXCVNAZoOI4K8f0o577IckQirhQmETUwqf85nfpo8Ndkc7n43mPDjSSlfuDsK'},
  params: {
    duration: 1
  }
})

instance.get('/devices/463/wakeUp').then((resp) => {
  console.log('resp: ', resp.data)
}).catch((error) => console.log(error))
