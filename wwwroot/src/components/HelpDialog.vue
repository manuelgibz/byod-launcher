<template>
    <div>
        <div id="helpView"></div>
        <div id="chatView"></div>
    </div>
</template>

<script lang="ts">
    import Vue from 'vue'
    import {HubConnectionBuilder} from "@microsoft/signalr";

    const connection = new HubConnectionBuilder().withUrl("https://localhost:5001/helpHub").build();
    connection.start();
    export default class HelpDialog extends Vue{
        sendHelpParams(){
            connection.invoke("SendMessage", "test", "test", "test", "test")
            console.log("true");
        }
    }
</script>

<style lang="scss">
    #helpView{
        top: 0;
        left: 0;
        position: fixed;
        height: 50vh;
        width: 300px;
        border-style: solid;
        border-right-width: 2px;
        border-color: #9e303c;
    }
    #chatView{
        bottom: 0;
        left: 0;
        position: fixed;
        height: 50vh;
        width: 300px;
        border-style: solid;
        border-right-width: 2px;
        border-color: #9e303c;
    }
    
 
</style>