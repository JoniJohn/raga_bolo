pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                git branch: 'develop',
                    credentialsId: 'github-creds',
                    url: 'https://github.com/JoniJohn/raga_bolo.git'
            }
        }

        stage('Deploy') {
            steps {
                withCredentials([file(credentialsId: 'raga_api-env', variable: 'ENV_FILE')]) {
                    sh '''
                        cp "$ENV_FILE" "$WORKSPACE/.env"
                        docker compose -f docker-compose.yml up -d --build --remove-orphans
                    '''
                }
            }
        }

        stage('Cleanup') {
            steps {
                sh 'docker image prune -f'
            }
        }
    }

    post {
        failure {
            withCredentials([file(credentialsId: 'raga_api-env', variable: 'ENV_FILE')]) {
                sh '''
                    cp "$ENV_FILE" "$WORKSPACE/.env"
                    docker compose up -d --no-build
                '''
            }
            echo 'Deploy failed — rolled back'
        }
        success {
            echo 'Deployment successful!'
        }
    }
}
