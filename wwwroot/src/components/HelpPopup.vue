<template>
    <div id="popup">
        <h2>Beschreiben Sie Ihr Problem:</h2>
        <p id="dpName">{{username}}</p>
        <v-textarea id="textarea" v-model="hmessage" cols="30" rows="5"></v-textarea>
        <button id="btnSend" v-on:click="sendHelpParams">Send</button>
    </div>
</template>



<script lang="ts">
    import Vue from 'vue'
    import {HubConnectionBuilder} from "@microsoft/signalr";
    import AttendSession from "@/views/AttendSession.vue";
    import OrchestrateSession from '@/views/OrchestrateSession.vue';
    
    const HelpPopupProps = Vue.extend({
    props: {
        username: String
    }
    
    })

    const connection = new HubConnectionBuilder().withUrl("https://localhost:5001/helpHub").build();
    connection.start();
    export default class HelpPopup extends HelpPopupProps{
        
        hmessage = "";
        
        
        sendHelpParams(){
            var area = document.getElementById("textarea");
            // console.log(area?.value);
            // console.log(this.message)
            var title = document.getElementById("step-title")?.innerHTML;
            connection.invoke("SendMessage", "Diese Nachricht wurde von der 'HelpPopup' Komponente gesendet. (Binding mit textarea funktioniert nicht)", document.getElementById("dpName")?.innerHTML, title)
            console.log("true");
            // AttendSession.popupActive = !AttendSession.popupActive;
            var btnHelp = document.getElementById("btnHelp");
            if(btnHelp) btnHelp.innerHTML = "?"
            var popup = document.getElementById("popup");
            if(popup) popup.style.display = "none"
            const chatPopup = document.getElementById("chatPopup")
                if(chatPopup) chatPopup.style.display = "none"
        }
    }
</script>

<style lang="scss">
    #popup{
        position: fixed;
        width: 70%;
        left: 15%;
        margin: auto;
        background-color: rgb(241, 241, 241);
        padding: 2rem;
        border-radius: 2rem;
        top: 25%;
        z-index: 25;
    }

    #textarea{
        font-size: 1rem;
        width: 100%;
        resize: none;
        border: 1px solid#9e303c;
        padding: 1rem;
    }
    input:focus, textarea:focus, select:focus, button:focus{
        outline: none;
    }
    #btnSend{
        border-radius: 2rem;
        background-color: #9e303c;
        border: none;
        float: right;
        padding: 1rem;
        color: white;
    }
</style>