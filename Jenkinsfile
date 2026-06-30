pipeline {
    agent any

    environment {
        DOCKER_IMAGE       = 'sravanreddy98/employee-api'
        DOCKER_TAG         = "${env.BUILD_NUMBER}"
        DOCKER_LATEST_TAG  = 'latest'
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
                bat 'dotnet restore EmployeeApi.sln'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build EmployeeApi.sln --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                script {
                    // returnStatus: true means exit code 0 = found, non-zero = not found
                    // Never fails the pipeline just because no test projects exist yet
                    def found = bat(
                        script: '@dir /s /b *Tests.csproj *Test.csproj 2>nul',
                        returnStatus: true
                    )

                    if (found == 0) {
                        bat 'dotnet test EmployeeApi.sln --configuration Release --no-build --logger "trx;LogFileName=test-results.trx"'
                        junit '**/test-results.trx'
                    } else {
                        echo 'No test projects found — skipping test stage.'
                    }
                }
            }
        }

        stage('Docker Build') {
            steps {
                bat """
                    docker build ^
                        -t %DOCKER_IMAGE%:%DOCKER_TAG% ^
                        -t %DOCKER_IMAGE%:%DOCKER_LATEST_TAG% ^
                        .
                """
            }
        }

        stage('Docker Push') {
            when {
                expression { env.GIT_BRANCH == 'origin/Main' }
            }
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-credentials',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    powershell """
                        \$ErrorActionPreference = 'Stop'

                        # Use a temp Docker config dir to bypass Docker Desktop
                        # credential helper (docker-credential-desktop) which
                        # conflicts when Jenkins runs as LocalSystem
                        \$env:DOCKER_CONFIG = "\$env:WORKSPACE\\.docker-tmp"
                        New-Item -ItemType Directory -Force -Path \$env:DOCKER_CONFIG | Out-Null

                        try {
                            \$env:DOCKER_PASS | docker login -u \$env:DOCKER_USER --password-stdin
                            docker push ${DOCKER_IMAGE}:${DOCKER_TAG}
                            docker push ${DOCKER_IMAGE}:${DOCKER_LATEST_TAG}
                        } finally {
                            Remove-Item -Recurse -Force \$env:DOCKER_CONFIG -ErrorAction SilentlyContinue
                        }
                    """
                }
            }
        }

    }

    post {
        always {
            bat """
                docker rmi %DOCKER_IMAGE%:%DOCKER_TAG% 2>nul || exit /b 0
                docker rmi %DOCKER_IMAGE%:%DOCKER_LATEST_TAG% 2>nul || exit /b 0
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
