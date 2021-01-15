<template> 
<div>
    <div id="helpBar">
        <button id="btnShow" v-on:click="btnShowClick">!</button>
        <div id="helpView">
            <ul id="helpList">
            </ul>
        </div>
        <div id="chatView">
            <ul id="schatList"></ul>
            <input v-model="stextMessage" id="stextBar" type="text">
            <button id="sbtnSendText" v-on:click="ssendTextMessage">Send</button>
        </div>
    </div>
    <v-container>
        
        <v-row>
            <v-col cols="12">
                <h1 class="display-2 mb-3">{{session.title}}</h1>

                <v-alert text
                         border="left"
                         icon="fa-exclamation"
                >
                    <p class="body-1 mb-0">
                        Sie können mit dem Code <span class="font-weight-bold">{{formattedAccessCode}}</span> an dieser
                        geführten Einrichtung teilnehmen.</p>
                </v-alert>

                <p v-if="Object.entries(participants).length > 0"
                   class="body-1 mt-10 mb-0"
                >
                    Aktuell Teilnehmende an dieser geführten Einrichtung:
                </p>

                <div>
                    <v-chip v-for="[participantId, displayName] in Object.entries(participants)"
                            :key="participantId"
                            class="ma-2"
                    >
                        {{displayName}}
                    </v-chip>
                </div>
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12">
                <stages-stepper>
                    <template v-slot:directors-stage-view>
                        <v-row justify="space-around"
                               class="py-2"
                        >
                            <v-btn v-if="stageNumber > 1"
                                   @click="$store.dispatch(orchestrateSessionIdentifiers.actions.changeStageNumber, -1)"
                                   small
                            >
                                <v-icon left>fa-caret-up</v-icon>
                                Zur vorherigen Stufe
                            </v-btn>
                            <v-btn v-if="stageNumber < stages.length"
                                   @click="$store.dispatch(orchestrateSessionIdentifiers.actions.changeStageNumber, 1)"
                                   small
                            >
                                Zur nächsten Stufe
                                <v-icon right>fa-caret-down</v-icon>
                            </v-btn>
                        </v-row>
                    </template>

                    <template v-slot:directors-target-view="{target}">
                        <target-results :target-id="target.id"/>
                    </template>
                </stages-stepper>
            </v-col>
        </v-row>

        <v-row v-show="stageNumber === 0">
            <v-col cols="12" class="text-center">
                <v-btn large
                       color="primary"
                       @click="startSession"
                >
                    <v-icon left small>fa-play</v-icon>
                    Geführte Einrichtung starten
                </v-btn>
            </v-col>
        </v-row>
    </v-container>
</div>
    
</template>


<script lang="ts">

    import {Vue, Component} from "vue-property-decorator";
    import StagesStepper from "@/components/StagesStepper.vue";
    import {mapState} from "vuex";
    import TargetResults from "@/components/TargetResults.vue";
    import {orchestrateSessionIdentifiers} from "@/store/newModules/orchestrateSession";
    import {HubConnectionBuilder} from "@microsoft/signalr";
    import HelpBar from "@/components/HelpBar.vue";

    var items = [];
    var i = 0;
    const connection = new HubConnectionBuilder().withUrl("https://localhost:5001/helpHub").build();
    connection.start();

    const chatConnection = new HubConnectionBuilder().withUrl("https://localhost:5001/chatHub").build();
    chatConnection.start();
    chatConnection.on("ReceiveTextMessage", (message, username)=>{
        const chatList = document.getElementById("schatList")
        var li = document.createElement("li")
        li.className = "schatItem"
        li.innerHTML += "<strong>" + username + "</strong>"
        li.innerHTML += "<p>" + message + "</p>"
        if(chatList) chatList.appendChild(li)
    });

    connection.on("AddHelp", (message, username, title, sessionID) => {
        const helpList = document.getElementById("helpList");
        console.log(message);
        var li = document.createElement("li");
        li.innerHTML += "<strong>" + username + "</strong>"
        li.innerHTML += "<p>" + message + "</p>"
        li.className = "helpListItem"
        if(helpList) helpList.appendChild(li);
    });
    function removeItem(id: number){
        // items.splice(items.indexOf(id), 1)
        console.log(id)
    }
    @Component({
        components: {TargetResults, StagesStepper},
        computed: {
            ...mapState('stage', {
                stages: 'stages',
            }),
            ...mapState('orchestrateSession', {
                participants: 'participants',
                stageNumber: 'stageNumber',
            })
        }
    })
    export default class OrchestrateSession extends Vue {
        get orchestrateSessionIdentifiers() {
            return orchestrateSessionIdentifiers;
        }

        stextMessage = ""
        ssendTextMessage(){
            var name = "Lehrer"
            chatConnection.invoke("SendTextMessage", this.stextMessage, name)
        }

        get session() {
            return this.$store.state.session.session;
        }

        get formattedAccessCode() {
            return this.session.accessCode.substr(0, 3) + "-" + this.session.accessCode.substr(3);
        }

        startSession() {
            this.$store.dispatch(orchestrateSessionIdentifiers.actions.startSession);
        }

        display = false
        btnShowClick(){
            this.display = !this.display;
            this.changeDisplayState()
        }

        changeDisplayState(){
                const helpView = document.getElementById("helpView");
                const chatView = document.getElementById("chatView");
                const btn = document.getElementById("btnShow");
            if(this.display){
                if(helpView) helpView.style.display = "block";
                if(chatView) chatView.style.display = "block";
                if(btn) btn.innerHTML = "-";
            }
            else{
                if(helpView) helpView.style.display = "none";
                if(chatView) chatView.style.display = "none";
                if(btn) btn.innerHTML = "!";
            }
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
        border-bottom: none;
        border-top: none;
        border-left: none;
        border-color: #9e303c;
        z-index: 24;
        background-color: white;
        display: none;
    }
    #chatView{
        bottom: 0;
        left: 0;
        position: fixed;
        height: 50vh;
        width: 300px;
        border-style: solid;
        border-right-width: 2px;
        border-bottom: none;
        border-top: none;
        border-left: none;
        border-color: #9e303c;
        z-index: 24;
        background-color: white;
        display: none;
    }
    #btnShow{
        background-color: #9e303c;
        width: 4rem;
        height: 4rem;
        border-radius: 2rem;
        color: white;
        position: fixed;
        font-size: 2rem;
        right: 4rem;
    }
    .helpListItem{
        border: 1px solid black;
        padding: 5px;
        margin-top: 5px;
        margin-right: 5px;
        border-radius: 5px;
    }
    #helpList{
        height: 100%;
        overflow-y: scroll;
    }

    #schatList{
        height: 90%;
        overflow-y: scroll;
        list-style-type: none;
    }

    #stextBar{
        bottom: 0;
        border: 1px solid #9e303c;
        top: 1%;
    }

    #sbtnSendText{
        border: none;
        top: 1%;
        bottom: 0;
        border: 1px solid #9e303c;
    }

    .schatItem{
        border: 1px solid #9e303c;
        border-radius: 5px;
        margin-top: 3px;
        padding: 5px;
    }
</style>
