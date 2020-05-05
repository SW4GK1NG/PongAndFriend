var io = require("socket.io")(5055);
var uid = require('uid');

console.log("socket listen at port 5055");

var playerIDList = [];
var Host;
var maxPlayer = 2;
var xpos = 0;

io.on("connection", (socket)=>{

    console.log("client connected : " + socket.id);

    ClientConnect(io, socket);
    ClientJoinGame(socket);

    socket.on("disconnect", ()=>{

        console.log("Client disconnected : " + socket.id);
        ClientDisconnect(io, socket);
        CountPlayer();
    });

    socket.on("Fwin", ()=> {
        socket.broadcast.emit("Fwon");
    });

    socket.on("Jwin", ()=> {
        socket.broadcast.emit("Jwon");
    });

    socket.on("PressF", ()=> {
        xpos -= 1;
        console.log(xpos);
        var data = {"position": xpos};
        io.emit("FPressed", data);
        console.log(socket.id + " pressed F")
    });

    socket.on("PressJ", ()=> {
        xpos += 1;
        console.log(xpos);
        var data = {"position": xpos};
        io.emit("JPressed", data);
        console.log(socket.id + " pressed J")
    });

    socket.on("Reset", ()=> {
        xpos = 0;
        socket.broadcast.emit("Reset");
        console.log("Reset Game");
    });
});

setInterval(()=> {

}, 10);

var ClientConnect = (io, socket)=> {
    var data = {"uid":socket.id};

    socket.emit("OnOwnerClientConnect", data);
}

var ClientDisconnect = (io, socket)=>{

    var data = {
        "uid":socket.id
    };

    for(var i = 0; i < playerIDList.length; i++)
    {
        if(playerIDList[i] == data.uid)
        {
            playerIDList.splice(i, 1);
            console.log("delete player : " + data.uid);
        }
    }

    io.emit("OnClientDisconnect", data);
}

var ClientJoinGame = (socket)=> {
    var data = {
        "uid":socket.id
    };
    playerIDList.push(data.uid);
    var temp = playerIDList.toString();
    console.log("Player Joined: " + data.uid);
    console.log("Player List: " + temp);
    CountPlayer();
};

var CountPlayer = ()=> {
    console.log("Player count: " + playerIDList.length);
    if (playerIDList.length <= 0) {
        xpos = 0;
        console.log("Reset Game");
    }
};

var GameFull = (socket)=> {
    socket.emit("GameIsFull");
    console.log("Client " + socket.id + "tried to join but game is full");
};

var ClientFetchPlayerList = (socket)=> {
    
    var data = {
        "playerIDList": playerIDList
    }
    socket.emit("OnClientFetchPlayerList", data);
    
};
