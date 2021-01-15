<template>

    
    <div>
        <button id="btnHelp" v-on:click="displayPopup">?</button>
        <help-popup id="popup" :username='displayName'></help-popup>


        <div id="chatPopup">
            <ul id="chatList"></ul>
            <input id="textBar" v-model="textMessage" type="text">
            <button id="btnSendText" v-on:click="sendTextMessage">Send</button>
        </div>

        <!-- TODO: Beim Page-Reload sollte nach Möglichkeit die gleiche Session automatisch wieder gestartet werden -->
        <div v-show="stages.length <= 0">LOADING...</div>

        <div v-show="stages.length > 0">
            <div class="text-center">
            <h1 class="display-3 mt-10 mb-2">{{session.title}}</h1>
            <p class="body-1 mb-1">Sie nehmen aktuell an der geführten Einrichtung mit dem Titel <span
                    class="font-weight-bold">{{session.title}}</span> teil.</p>
            <p class="body-1 mb-10">Ihr Anzeigename lautet <span id="dName" class="font-weight-bold">{{displayName}}</span>.</p>
            </div>
            <stages-stepper>
                <template v-slot:participant-target-view="{target}">
                    <v-divider class="my-3"/>
                    <v-row v-if="Object.hasOwnProperty.call(target, 'stepIds')"
                           justify="space-around"
                    >
                        <v-btn-toggle v-model="targetResultToggleButton">
                            <v-btn @click="sendTargetResult(target.id, true)"
                                   small outlined color="primary"
                            >
                                Ausführung erfolgreich
                            </v-btn>
                            <v-btn @click="sendTargetResult(target.id, false)"
                                   small outlined color="primary"
                            >
                                Ausführung fehlgeschlagen
                            </v-btn>
                        </v-btn-toggle>
                    </v-row>
                </template>
            </stages-stepper>
        </div>
    </div>
</template>

<script lang="ts">
    import {Vue, Component} from "vue-property-decorator";
    import {mapState} from "vuex";
    import StagesStepper from "@/components/StagesStepper.vue";
    import HelpPopup from "@/components/HelpPopup.vue";
    import {Id} from "@/models/idType";
    import {attendSessionIdentifiers} from "@/store/newModules/attendSession";
    import {HubConnectionBuilder} from "@microsoft/signalr";

    const connection = new HubConnectionBuilder().withUrl("https://localhost:5001/helpHub").build();
    connection.start();

    const chatConnection = new HubConnectionBuilder().withUrl("https://localhost:5001/chatHub").build();
    chatConnection.start();
    chatConnection.on("ReceiveTextMessage", (message, username)=>{
        const chatList = document.getElementById("chatList")
        var li = document.createElement("li")
        li.className = "chatItem"
        li.innerHTML += "<strong>" + username + "</strong>"
        li.innerHTML += "<p>" + message + "</p>"
        if(chatList) chatList.appendChild(li)
    });


    @Component({
        components: {StagesStepper, HelpPopup},
        computed: {
            ...mapState('session', {
                session: 'session',
            }),
            ...mapState('stage', {
                stages: 'stages',
            }),
            ...mapState('attendSession', {
                displayName: 'displayName',
            }),
        }
    })
    export default class AttendSession extends Vue {
        targetResultToggleButton = NaN;

        textMessage = "";
        sendTextMessage(){
            var name = document.getElementById("dName")?.innerHTML
            chatConnection.invoke("SendTextMessage", this.textMessage, name)
        }

        created() {
            window.addEventListener('beforeunload', this.preventTabClose);
        }

        preventTabClose(event: BeforeUnloadEvent) {
            event.preventDefault();
            event.returnValue = "";
            return "";
        }

        sendTargetResult(targetId: Id, success: boolean) {
            this.$store.dispatch(attendSessionIdentifiers.actions.sendTargetResult, {targetId, success});
        }

        helpRequestActive = false;
        public static popupActive = false;
        displayPopup(){
            AttendSession.popupActive = !AttendSession.popupActive;
            this.displayChange();
        }
        displayChange(){
            const popup = document.getElementById('popup');
            const btnHelp = document.getElementById("btnHelp");
            const chatPopup = document.getElementById("chatPopup")
            

            if(AttendSession.popupActive){
                if(popup) popup.style.display = "block";
                
                if(btnHelp) btnHelp.innerHTML = "X";

                if(chatPopup) chatPopup.style.display = "block"
            }
            else{
                if(popup) popup.style.display = "none";
                if(btnHelp) btnHelp.innerHTML = "?";
                if(chatPopup) chatPopup.style.display = "none"
            }
        }

        
    }
</script>

<style lang="scss">

    .v-stepper__step__step .v-icon.v-icon {
        font-size: 0.95rem;
    }

    .v-stepper__step__step {
        font-size: 0.85rem;
    }

    #btnHelp{
        background-color: #9e303c;
        width: 4rem;
        height: 4rem;
        border-radius: 2rem;
        color: white;
        position: fixed;
        font-size: 2rem;
        right: 4rem;
    }
    #popup{
        display: none;
    }

    #chatPopup{
        position: fixed;
        height: 50vh;
        right: 0;
        top: 25vh;
        width: 12vw;
        background-color: white;
        z-index: 24;
        border: 2px solid #9e303c;
        display: none;
    }

    #chatList{
        height: 80%;
        overflow-y: scroll;
        list-style-type: none;
    }

    #textBar{
        bottom: 0;
        border: 1px solid #9e303c;
        top: 1%;
    }

    #btnSendText{
        border: none;
        top: 1%;
        bottom: 0;
        border: 1px solid #9e303c;
    }

    .chatItem{
        border: 1px solid #9e303c;
        border-radius: 5px;
        margin-top: 3px;
        padding: 5px;
    }
</style>