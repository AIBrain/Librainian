// ShoutStage Developer Interview Challenge
// source by Rick Harker, version 0.1

var Config = new Parameters(100, 5000, 10000);

var startingTime = new Date().getTime();

var events = require('events');
var eventEmitter = new events.EventEmitter();

// Begin
var timerRefs = [];
//when [does?] nodejs do garbage collection?

String.prototype.format = function() {
    var formatted = this;
    for (arg in arguments) {
        formatted = formatted.replace("{" + arg + "}", arguments[arg]);
    }
    return formatted;
};

// Configurable parameters.

function Parameters(total, shortest, longest) {
    this.TotalTimers = total;
    this.ShortestTimer = shortest;
    this.LongestTimer = longest;
}

function random(min, max) {
    if (max < min) {
        var tmp = max;
        max = min;
        min = tmp;
    }
    var length = max - min;
    return min + (Math.random() * length);
}

var DisplayID = 0;

function elapsed(createID) {
    DisplayID++;
    var rand = random(0, 100).toFixed(0);
    console.log("Random={0}\tCreateId={1}\tDisplayId={2}".format(rand, createID, DisplayID));
    if (DisplayID >= Config.TotalTimers) {
        eventEmitter.emit('endDemo');
    }
}

eventEmitter.on('startDemo', function() {
    for (var createid = 0; createid < Config.TotalTimers; createid++) {
        clearTimeout(timerRefs[createid]);
        var delay = random(Config.ShortestTimer, Config.LongestTimer);
        timerRefs[createid] = setTimeout(elapsed, delay, createid);
    }
});

eventEmitter.on('endDemo', function() {
    var endingTime = new Date().getTime();
    var elapsedTime = (endingTime - startingTime) / 1000.0;
    console.log("Total time {0} seconds.".format(elapsedTime.toFixed(2)));
});

eventEmitter.emit('startDemo');