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
    id = None
    space_rows = 0
    space_cols = 0
    success = False
    failureReason = None
    errorCode = None
    row = 0
    col = 0

    def __init__(self, id):
        self.id = id

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
            return ["clean", id, row, col]
        else:
            r, c = self.NewLocation(row, col)
            return ["moveto", id, r, c]

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


@app.route('/<int:id>/init', methods=['GET'])
def init(id):
    if(request.method == 'GET'):
        print('agent #' + str(id) + ' initialized')
        data = 'agent #' + str(id) + ' initialized'
        agents[id] = Agent(id)
        return jsonify({'message': data})


@app.route('/<int:id>/spacesize', methods=['GET'])
def init(id):
    if(request.method == 'GET'):
        r = request.args.get('rows')
        c = request.args.get('cols')
        data = 'agent #' + str(id) + ' space received'
        agents[id].SetSpaceSize(r, c)
        return jsonify({'message': data})


@app.route('/<int:id>/commandresult', methods=['GET'])
def init(id):
    if(request.method == 'GET'):
        s = False if int(request.args.get('success')) == 0 else True
        freason = request.args.get('failureReason')
        err = request.args.get('errorCode')
        r = request.args.get('row')
        c = request.args.get('col')
        data = 'agent #' + str(id) + ' result received'
        agents[id].CommandResult(s, freason, err, r, c)
        return jsonify({'message': data})


@app.route('/<int:id>/nextcommand', methods=['GET'])
def init(id):
    if(request.method == 'GET'):
        isDirty = False if int(request.args.get('isDirty')) == 0 else True
        r = request.args.get('row')
        c = request.args.get('col')
        data = 'agent #' + str(id) + ' result received'
        cmd = agents[id].NextCommand(r, c, isDirty)
        return jsonify(cmd)

# driver function
if __name__ == '__main__':
    app.run(debug=True)
