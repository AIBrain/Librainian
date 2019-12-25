var textColor = function (bgColor) {
    var output = runNetwork(bgColor);
    if (output.black > .5) {
        return 'black';
    }
    return 'white';
};
var runNetwork = function anonymous(inputs) {
    var net = {
        "layers": [{
            "r": {},
            "g": {},
            "b": {}
        }, {
            "0": { "bias": -1.1919392246276248, "weights": { "r": 1.0182930135696358, "g": 2.4442490696399206, "b": 0.23685104908207724 } },
            "1": { "bias": 2.2458707249645733, "weights": { "r": -1.3999326881609242, "g": -3.682874462617662, "b": -0.07853163907719128 } },
            "2": { "bias": 3.480106154563392, "weights": { "r": -1.9086180482415893, "g": -5.034352421600551, "b": -0.30002228072765996 } }
        }, {
            "black": {
                "bias": 1.7857413101740096,
                "weights": { "0": 3.916650554668547, "1": -4.66550909186612, "2": -6.641976845137061 }
            }
        }]
    };

    for (var i = 1; i < net.layers.length; i++) {
        var layer = net.layers[i];
        var outputs = {};
        for (var id in layer) {
            var node = layer[id];
            var sum = node.bias;
            for (var iid in node.weights)
                sum += node.weights[iid] * inputs[iid];
            outputs[id] = (1 / (1 + Math.exp(-sum)));
        }
        inputs = outputs;
    }
    return outputs;
};