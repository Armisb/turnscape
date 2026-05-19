import http from 'k6/http';
import ws from 'k6/ws';
import { sleep, check } from 'k6';

export const options = {
  vus: 4,
  duration: '50s',
};

function makestr(length){
  var result = '';
  var chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  for ( var i = 0; i < length; i++){
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
}

export default function() {
  let data = { UserName: makestr(20), Password: makestr(20) };
  let res = http.post('https://localhost:7232/user/signup', JSON.stringify(data), {
    headers: { 'Content-Type': 'application/json' },
  });
  let userId = res.json().id;
  res = http.post('https://localhost:7232/user/login', JSON.stringify(data), {
    headers: { 'Content-Type': 'application/json' },
  });

  let accessToken = res.json().accessToken;

  ws.onopen = () => {
    console.log('WebSocket connection established!');
    ws.close();
  };
  const resp = ws.connect('wss://localhost:7232/matchhub?userid=' + userId, { headers: { 'Authorization': 'Bearer ' + accessToken } }, function (socket) {
    socket.on('open', () => {
      socket.send('{"protocol":"json","version":1}\x1e');
      console.log('open');
      res = http.post('https://localhost:7232/match/lobby/' + userId, "", {
        headers: { 'Authorization': 'Bearer ' + accessToken, 'Content-Type': 'application/json' },
      });
    });
    socket.on('message', (message) => {
      try {
        packet = JSON.parse(message.slice(0, -1));
        console.log('packet: ', packet);
      } catch {
        console.log(`message: ${message}`)
      }
    });
    socket.on('close', () => console.log('close')); 
  });
  console.log(resp);
  check(resp, { 'status is 101': (r) => r && r.status === 101 });
  sleep(3);
}
