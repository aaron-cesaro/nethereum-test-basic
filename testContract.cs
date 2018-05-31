﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Hex = Nethereum.Hex.HexTypes;
using Nethereum.Web3;

using Nethereum.RPC.Eth.DTOs;

namespace nethereum_test2
{
    class testContract
    {
        public async void TestToken()
        {

            /* ### DEFINING CONTRACT ### */

            /* contract bytecode (for EVM) */
            var byteCode = "608060405234801561001057600080fd5b506040516020806102f18339810160409081529051600160a060020a0333166000908152600160205291822081905590556102a1806100506000396000f3006080604052600436106100615763ffffffff7c010000000000000000000000000000000000000000000000000000000060003504166318160ddd811461006657806323b872dd1461008d57806370a08231146100cb578063a9059cbb146100ec575b600080fd5b34801561007257600080fd5b5061007b610110565b60408051918252519081900360200190f35b34801561009957600080fd5b506100b7600160a060020a0360043581169060243516604435610116565b604080519115158252519081900360200190f35b3480156100d757600080fd5b5061007b600160a060020a03600435166101b8565b3480156100f857600080fd5b506100b7600160a060020a03600435166024356101d3565b60005490565b600160a060020a03831660009081526001602052604081205482116101ad57600160a060020a0380841660008181526001602090815260408083208054880190559388168083529184902080548790039055835191825281019190915280820184905290517f5225eac2a7facfbeb099c00a4cc7c457701324f1fd84b84b14033f9e911374a49181900360600190a15060016101b1565b5060005b9392505050565b600160a060020a031660009081526001602052604090205490565b600160a060020a033316600090815260016020526040812054821161026b57600160a060020a038084166000818152600160209081526040808320805488019055339094168083529184902080548790039055835191825281019190915280820184905290517f5225eac2a7facfbeb099c00a4cc7c457701324f1fd84b84b14033f9e911374a49181900360600190a150600161026f565b5060005b929150505600a165627a7a723058206a23128e894c8fd1e4ae0072bcd71e94ab92b4341d158a9b976f1ffe9475797e0029";
            /* ABI (contract interface definition) */
            var abi = @"[{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":""supply"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_from"",""type"":""address""},{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""name"":""success"",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""balance"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""name"":""success"",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[{""name"":""_initialAmount"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""_from"",""type"":""address""},{""indexed"":false,""name"":""_to"",""type"":""address""},{""indexed"":false,""name"":""_value"",""type"":""uint256""}],""name"":""Tranfer"",""type"":""event""}]";
            System.Console.WriteLine("Contract defined");

            /* ### DEFINING ACCOUNT ### */

            /* a private key is needed to sign the transactions.
               The ethereum address is calculated from this private key 
               so each transaction signed with this key can be related to our ethereum addess*/
            var privateKey = "dc925af9892df9f5f6f00dcbee5f6c59d53b631b32b475dfec49fabed8f57510";
            /* Who can do transactions, signing them with his private key */
            var senderAddress = "0x67af396FB49b054ad946a9E2cd67123750Fa1207";
            /* now it is possible to create an instance of Account which will be used to sign the transactions*/
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            /* An instance of Web3 must be created to interact to the Ethereum client via RPC (remote procedure call).
               [constructor public Web3(IAccount account, IClient client)] [client use ganache default port 8545] */
            var web3 = new Web3(account, "HTTP://127.0.0.1:8545");
            System.Console.WriteLine("Account defined");

            /* ### DEPLOYING CONTRACT ### */

            /* creating _totalSupply to initialize SGM contract 
               I'm using BigInteger to pass the amount in wei (1 ETH = 10^18 wei)*/
            System.Numerics.BigInteger totalSupply = System.Numerics.BigInteger.Parse("5000000000000000000");
            /* Now we deploy the smart contract using SGM's abi and bytecode.
               The method send the initial transaction (SendRequest), aka call the SGM constructor, 
               waits for the trasaction to be mined (AndAwait) and finally returns the transaction receipt.
               The transaction receipt of a newly deployed contract includes the contract's address.
               This address can be now used to interact with the contract itself. */
            var receipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(
                abi,                                              //contract's interface definition
                byteCode,                                         //contract's  bytecode
                senderAddress,                                    //deployer
                new Nethereum.Hex.HexTypes.HexBigInteger(900000), //gas
                null,
                totalSupply);                                     //SGM contructor parameter
            System.Console.WriteLine("Contract deployed");

            /* ### INTERACTING WITH THE CONTRACT ### */

            System.Console.WriteLine("Retrieving contract functions");
            /* Now we'll create an instance of a contract with the ABI and the contract's 
               address stored in receipt variable */
            var contract = web3.Eth.GetContract(abi, receipt.ContractAddress);
            /* from now we can use contract's functions to interact with the smart contract, 
               using their names */
            var balanceFunction = contract.GetFunction("balanceOf");
            var transferFunction = contract.GetFunction("transfer");
            var transferFromFunction = contract.GetFunction("transferFrom");
            /* declaring data to be passed to contract function;
               in this case to transfer funds we need a beneficiary 
               and an amount to send. */
            var addressTo = "0x3B4D0bD5F3bBe92b5176FC2540Ca9A873737e1B8";
            var amountToSend = 10000;
            /* befor calling the contract function is better to estimate 
               the gas consumption. In this way the gas variable can be 
               passed to SendTransactionAndWaitForReceiptAsync function */
            var gas = await transferFromFunction.EstimateGasAsync(senderAddress, addressTo, amountToSend);
            /* Now SendTransactionAndWaitForReceiptAsync will wait for the transaction 
               to be mined and included in the chain (Ganache use automining) */
            var counter = 0;
            Task<TransactionReceipt>[] transactions = new Task<TransactionReceipt>[31];
            while (counter < 31)    //loop used to demonstrate more than one transaction at the time
            {
                System.Console.WriteLine("Estimated gas: " + 
                                         gas.Value + 
                                         "\nEstimated cost: " +
                                         ((Decimal)gas.Value * (Decimal)0.000000003 * 675/*price at 05-18-2018*/) + 
                                         " $" +
                                         "\nEstimated cost: " +
                                         ((Decimal)gas.Value * (Decimal)0.000000003* 572/*price at 05-18-2018*/) + 
                                         " Euro");
                /* Here's an example with balanceOf function, used to query the state of
                   the smart contract */
                var senderBalance = await balanceFunction.CallAsync<System.Numerics.BigInteger>(senderAddress);
                var receiverBalance = await balanceFunction.CallAsync<System.Numerics.BigInteger>(addressTo);
                //System.Console.WriteLine("PRE Sender balance: " + senderBalance.ToString());
                System.Console.WriteLine("Receiver balance: " + receiverBalance.ToString());
                /* SendTransactionAndWaitForReceiptAsync is used to call smart contract's tranfer method */
                if(counter % 2 == 0) {
                    transactions[counter] = transferFunction.SendTransactionAndWaitForReceiptAsync(
                    "INVALID ACCOUNT",
                    gas,
                    null,
                    null,
                    addressTo,          //transfer function parameter
                    amountToSend);      //transfer function parameter
                } else {
                    transactions[counter] = transferFunction.SendTransactionAndWaitForReceiptAsync(
                        senderAddress,
                        gas,
                        null,
                        null,
                        addressTo,          //transfer function parameter
                        amountToSend);      //transfer function parameter
                }

                /*var receiptSecondAmountSend = transferFromFunction.SendTransactionAndWaitForReceiptAsync(
                    senderAddress,
                    gas,
                    null,
                    null,
                    senderAddress,      //transfer function parameter
                    addressTo,          //transfer function parameter
                    amountToSend);      //transfer function parameter
                
                Thread.Sleep(500);      //used to slow down the process (human readable)*/
                counter++;
                Thread.Sleep(500);
            }
            System.Console.WriteLine("Tranfer function DONE, " + amountToSend*counter + " transferred");
            int i = 0;
            foreach (Task<TransactionReceipt> tx in transactions) {
                try {
                    if(tx.Result.Status.Value == 1) {
                        System.Console.WriteLine(i + ") Tx: " + tx.Result.TransactionHash + " DONE");
                        System.Console.WriteLine(tx.Result.Logs);
                    }
                } catch (System.AggregateException e) {
                    System.Console.WriteLine("\n" + i + ") Tx FAILED " + e.Message + "XXXXXXXXXXXXXXX\n");
                }
                i++; 
            }
            System.Console.WriteLine("########################");            
            /*foreach (Task<TransactionReceipt> tx in transactions)
                System.Console.WriteLine(tx.Result.Logs);
            */
        }

        static void Main(string[] args)
        {
            testContract test = new testContract();
            test.TestToken();
            System.Console.ReadKey();
        }
    }
}