pragma solidity ^0.4.17;

contract SGM {
    uint _totalSupply;
    mapping(address => uint) balances;

    event Tranfer(address _from, address _to, uint _value);

    constructor(uint _initialAmount) public {
        balances[msg.sender] = _initialAmount;
        _totalSupply = _initialAmount;
    }

    function totalSupply() public constant returns (uint supply) {
        return _totalSupply;
    }

    function balanceOf(address _owner) public constant returns (uint balance) {
        return balances[_owner];
    }

    function transfer(address _to, uint _value) public returns (bool success) {
        if(balances[msg.sender] >= _value) {
            balances[_to] += _value;
            balances[msg.sender] -= _value;
            emit Tranfer(msg.sender, _to, _value);
            return true;
        } return false;
    }
    function transferFrom(address _from, address _to, uint _value) public returns (bool success) {
        if(balances[_from] >= _value) {
            balances[_to] += _value;
            balances[_from] -= _value;
            emit Tranfer(_from, _to, _value);
            return true;
        } return false;
    }
}