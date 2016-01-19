# TRPG-server
server for the universal tabletop roleplaying game system

S->HH,TT    turn-start

HH->S->TT   action-selected
TT->S->HH   action-received
HH->S->TT   target-selected
HH->S->TT   target-confirmed
TT->S->HH   action-resolved

TT->S       movement-initiated
TT->S       movement-selected
HH->S       movement-confirmed

TT->S       turn-end

MAP DATA

{
  "walls": [
    {
      "x":0,
      "y":0,
      "pos":1
    }
  ]
}

WALL POSITION:
  1
  *
4*x*2
  *
  3
