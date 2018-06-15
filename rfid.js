const http = require('http')
const net = require('net')
const url = require('url')
const fs = require('fs')

const APPARATUS_ID = 'E4'
const DEBUG = false

// Create an HTTP tunneling proxy
const proxy = http.createServer((req, res) => {
  res.writeHead(200, { 'Content-Type': 'text/plain' })
  res.end('okay')
})
proxy.on('connect', (req, cltSocket, head) => {
  // connect to an origin server
  const srvUrl = url.parse(`http://${req.url}`)
  const srvSocket = net.connect(srvUrl.port, srvUrl.hostname, () => {
    cltSocket.write('HTTP/1.1 200 Connection Established\r\n' +
                    'Proxy-agent: Node.js-Proxy\r\n' +
                    '\r\n')
    srvSocket.write(head)
    srvSocket.pipe(cltSocket)
    cltSocket.pipe(srvSocket)
  })
})

// now that proxy is running
proxy.listen(1337, '127.0.0.1', () => {
  // make a request to a tunneling proxy
  const options = {
    port: 1337,
    hostname: '127.0.0.1',
    method: 'CONNECT',
    path: '10.10.100.100:8899'
  }

  const req = http.request(options)
  req.end()

  let usersObject = {users: []}

  req.on('connect', (res, socket, head) => {
    console.log('got connected!')
    // make a request over an HTTP tunnel
    socket.write('GET / HTTP/1.1\r\n' +
                 'Host: 10.10.100.100:8899\r\n' +
                 'Connection: close\r\n' +
                 '\r\n')
    socket.on('data', (chunk) => {
      let rawInfo = JSON.parse(JSON.stringify(chunk))

      if (rawInfo.data.length === 16) {
	// TODO: with many users present, rawInfo.data might be longer than 16.
	// In that case, it will be either 16, 32, 48, etc.
	// slice(12) will still work, but
	// it would be better to take the rawInfo.data array, chunk into lengths of 16,
	// and then slice(12) each of those chunks

	// create a userId from the raw data i.e. 12-34-56-789
	let userInfo = rawInfo.data.slice(12)
	userId = userInfo.map(num => num + '-').join('').slice(0, -1)

	let allUserIds = usersObject.users.map(user => user.userId)

	// is userId already in userObject.userId?
	if (allUserIds.includes(userId)) {
	  // if true, then find the index position of the user and update lastTimeStamp  
          let idx = allUserIds.indexOf(userId)
	  usersObject.users[idx].lastTimeStamp = Date.now()
	} else {
	  // if false, then add the new user
	  usersObject.users.push({
	    'userId': userId,
	    'firstTimeStamp': Date.now(),
	    'lastTimeStamp': Date.now(),
	    'apparatusId': APPARATUS_ID
	  })
	}
      }
      if (DEBUG) { console.log('usersObject.users: ', usersObject.users) }

      fs.writeFile('rfid-data.txt', JSON.stringify(usersObject.users), (err) => {
	  if (err) throw err;
	  console.log('The file has been saved!');
      })

    })
    socket.on('end', () => {
      proxy.close()
    })
  })
})
