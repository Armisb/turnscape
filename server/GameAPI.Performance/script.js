import http from 'k6/http';
import ws from 'k6/ws';
import { sleep, check } from 'k6';

export const options = {
  vus: 20,
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
    let match_id = null;
    socket.on('open', () => {
      socket.send('{"protocol":"json","version":1}\x1e');
      console.log('open');
      res = http.post('https://localhost:7232/match/lobby/' + userId, "", {
        headers: { 'Authorization': 'Bearer ' + accessToken, 'Content-Type': 'application/json' },
      });
    });
    socket.on('message', (message) => {
      console.log('message: ' + message.slice(0, -1));
      message = message.split('\x1e')[0];
      let invocation_id = 0;
      let packet = JSON.parse(message);
      if (packet.type == null) {
        console.log('empty packet');
      } else if (packet.type == 1) {
        if (packet.target == "MatchFound" || packet.target == "MatchUpdated") {
          if (packet.arguments[0].isFinished == true) {
            socket.close();
            return;
          }
          match_id = packet.arguments[0].id;
          if (userId == packet.arguments[0].currentTurnPlayerId) {
            let send_msg = '{"type":1,"invocationId":"'+invocation_id+'","target":"Attack","arguments":["'+match_id+'","lite"]}\x1e';
            invocation_id++;
            console.log('sending message: ', send_msg);
            socket.send(send_msg);
          }
        }
      } else if (packet.type == 6) {
        socket.send('{"type":6}\x1e');
      } else {
        console.log('type: ', packet.type);
      }
    });
    socket.on('close', () => console.log('close')); 
  });
  console.log(resp);
  check(resp, { 'status is 101': (r) => r && r.status === 101 });
  sleep(3);
}
