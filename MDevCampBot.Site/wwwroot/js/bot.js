window.onload = () => {
    startButton.style.display = "none";
    botConnection.postActivity({ type: "event", name: "startGame", from: {id: "Visitor" }}).subscribe(_ => console.log("startGame sent"));
  }
  
  var botConnection = new BotChat.DirectLine({secret: "1D2kyxMpMIg.cwA.u3g.BhNuI2xdxjSQgP_KiPYM0tV7VZ8RqyIoK2Q6_EqatYo"});
  
  BotChat.App({
    botConnection: botConnection,
    user: { id: 'Visitor' },
    bot: { id: 'Bot' },
    resize: 'detect'
  }, document.getElementById("bot"));

  document.getElementsByClassName("wc-header")[0].innerHTML = "<span>Who is that?</span>";

  // element handlers
  var startButton = document.getElementById("startButton");
  startButton.onclick = () => {
    location.reload();
    //startButton.style.display = "none";
    //botConnection.postActivity({ type: "event", name: "startGame", from: {id: "Visitor" }}).subscribe(_ => console.log("startGame sent"));
  }

  botConnection.activity$
    .filter(activity => activity.type == "event")// && activity.name == "endGame")
    .subscribe(activity => handleEvent(activity));

  function handleEvent(event) {
    if (event.name == "endGame") {
      setFinalScore(event.value);
      startButton.style.display = "block";
    }

    if (event.name == "scoreUpdate") {
        document.getElementById("scoreView").innerHTML = `Score so far: ${event.value}`;
    }
  }

  function setFinalScore(score) {
    document.getElementById("finalScore").innerHTML = `Your final score: <b>${score}</b>.`;
  }