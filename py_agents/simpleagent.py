from flask import Flask, jsonify, request
from enum import Enum
import random
app = Flask(__name__)

agents = {}


class Direction(Enum):
    N = 1
    E = 2
    W = 3
    S = 4


class Agent:
    _id = None
    space_rows = 0
    space_cols = 0
    success = False
    failureReason = None
    errorCode = None
    row = 0
    col = 0

    def __init__(self, i):
        self._id = i

    def SetSpaceSize(self, r, c):
        self.space_rows = r
        self.space_cols = c

    def CommandResult(self, success, failureReason, errorCode, row, col):
        self.success = success
        self.failureReason = failureReason
        self.errorCode = errorCode
        self.row = row
        self.col = col

    def NextCommand(self, row, col, isDirty):
        if isDirty:
            return ["clean", self._id, row, col]
        else:
            r, c = self.NewLocation(row, col)
            return ["moveto", self._id, r, c]

    def NewLocation(self, row, col):
        # move in a random direction
        direction = Direction(random.randint(1, 4))

        if direction is Direction.E:
            return (row, col + 1)
        elif direction is Direction.W:
            return (row, col - 1)
        elif direction is Direction.N:
            return (row - 1, col)
        elif direction is Direction.S:
            return (row + 1, col)
        return (row, col)


@app.route('/', methods=['GET'])
def home():
    if(request.method == 'GET'):
        data = "this is a simple cleaning agent ai"
        return jsonify({'message': data})


@app.route('/<int:_id>/init', methods=['GET'])
def init(_id):
    if(request.method == 'GET'):
        print('agent #' + str(_id) + ' initialized')
        data = 'agent #' + str(_id) + ' initialized'
        agents[_id] = Agent(_id)
        return jsonify({'message': data})


@app.route('/<int:_id>/spacesize', methods=['GET'])
def spaceSize(_id):
    if(request.method == 'GET'):
        r = request.args.get('rows')
        c = request.args.get('cols')
        data = 'agent #' + str(_id) + ' space received'
        agents[_id].SetSpaceSize(r, c)
        return jsonify({'message': data})


@app.route('/<int:_id>/commandresult', methods=['GET'])
def commandResult(_id):
    if(request.method == 'GET'):
        s = False if int(request.args.get('success')) == 0 else True
        freason = request.args.get('failureReason')
        err = request.args.get('errorCode')
        r = request.args.get('row')
        c = request.args.get('col')
        data = 'agent #' + str(_id) + ' result received'
        agents[_id].CommandResult(s, freason, err, int(r), int(c))
        return jsonify({'message': data})


@app.route('/<int:_id>/nextcommand', methods=['GET'])
def nextCommand(_id):
    if(request.method == 'GET'):
        isDirty = False if int(request.args.get('isdirty')) == 0 else True
        r = request.args.get('row')
        c = request.args.get('col')
        data = 'agent #' + str(_id) + ' result received'
        cmd = agents[_id].NextCommand(int(r), int(c), isDirty)
        print(cmd)
        return jsonify(cmd)

# driver function
if __name__ == '__main__':
    app.run(debug=True)
