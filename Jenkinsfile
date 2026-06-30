pipeline {
    agent any

    environment {
        // Change DOCKER_IMAGE to your Docker Hub username/repo name
        DOCKER_IMAGE      = 'your-dockerhub-username/employee-api'
        DOCKER_TAG        = "${env.BUILD_NUMBER}"
        DOCKER_LATEST_TAG = 'latest'

        // 'dockerhub-credentials' is the ID of the credential you save in
        // Jenkins → Manage Jenkins → Credentials (Username + Password)
        DOCKER_CREDENTIALS = 'dockerhub-credentials'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
                echo "Branch: ${env.BRANCH_NAME} | Build: ${env.BUILD_NUMBER}"
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore EmployeeApi.sln'
            }
        }

        stage('Build') {
            steps {
                sh '''
                    dotnet build EmployeeApi.sln \
                        --configuration Release \
                        --no-restore
                '''
            }
        }

        stage('Test') {
            steps {
                script {
                    // Finds every *Tests.csproj automatically.
                    // Add test projects under tests/ and they are picked up here.
                    def testProjects = sh(
                        script: "find . -name '*Tests.csproj' -o -name '*Test.csproj'",
                        returnStdout: true
                    ).trim()

                    if (testProjects) {
                        sh '''
                            dotnet test EmployeeApi.sln \
                                --configuration Release \
                                --no-build \
                                --logger "trx;LogFileName=test-results.trx"
                        '''
                        // Publish test results in Jenkins UI
                        junit '**/test-results.trx'
                    } else {
                        echo 'No test projects found — skipping test stage.'
                    }
                }
            }
        }

        stage('Docker Build') {
            steps {
                sh """
                    docker build \
                        -t ${DOCKER_IMAGE}:${DOCKER_TAG} \
                        -t ${DOCKER_IMAGE}:${DOCKER_LATEST_TAG} \
                        .
                """
            }
        }

        stage('Docker Push') {
            when {
                // Only push when building the main branch
                branch 'main'
            }
            steps {
                withCredentials([usernamePassword(
                    credentialsId: "${DOCKER_CREDENTIALS}",
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh """
                        echo "${DOCKER_PASS}" | docker login -u "${DOCKER_USER}" --password-stdin
                        docker push ${DOCKER_IMAGE}:${DOCKER_TAG}
                        docker push ${DOCKER_IMAGE}:${DOCKER_LATEST_TAG}
                    """
                }
            }
        }

    }

    post {
        always {
            // Clean up local Docker images to free disk space on the agent
            sh """
                docker rmi ${DOCKER_IMAGE}:${DOCKER_TAG}    || true
                docker rmi ${DOCKER_IMAGE}:${DOCKER_LATEST_TAG} || true
            """
            cleanWs()
        }
        success {
            echo "Build ${env.BUILD_NUMBER} succeeded. Image: ${DOCKER_IMAGE}:${DOCKER_TAG}"
        }
        failure {
            echo "Build ${env.BUILD_NUMBER} FAILED. Check the logs above."
        }
    }
}
